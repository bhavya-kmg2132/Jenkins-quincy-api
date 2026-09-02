using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Common;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours
{
    public class FieldPermissionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<TRequest> _logger;

        // All caches populated once per type for the lifetime of the app
        private static readonly ConcurrentDictionary<Type, List<(PropertyInfo Prop, string EditPermission, string? ViewPermission, bool ThrowError)>>
            _editCache = new ConcurrentDictionary<Type, List<(PropertyInfo Prop, string EditPermission, string? ViewPermission, bool ThrowError)>>();

        private static readonly ConcurrentDictionary<Type, List<(PropertyInfo Prop, string Permission)>>
            _viewCache = new ConcurrentDictionary<Type, List<(PropertyInfo Prop, string Permission)>>();

        private static readonly ConcurrentDictionary<Type, List<CollectionMeta>>
            _collectionCache = new ConcurrentDictionary<Type, List<CollectionMeta>>();

        private readonly ICurrentUserService _currentUserService;

        public FieldPermissionBehaviour(ICurrentUserService currentUserService, ILogger<TRequest> logger)
        {
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // EDIT CHECK — on the way IN, before handler runs
            await CheckEditPermissions(request);

            var response = await next();

            // VIEW CHECK — on the way OUT, after handler returns
            if (response != null)
                await MaskViewPermissions(response);

            return response;
        }

        // ── EDIT ──────────────────────────────────────────────────────────────

        private async Task CheckEditPermissions(TRequest request)
        {
            var props = _editCache.GetOrAdd(typeof(TRequest), GetEditProperties);

            if (props.Count == 0)
                return;

            var distinctPerms = props
                .SelectMany(p => new[] { p.EditPermission, p.ViewPermission })
                .Where(p => p != null)
                .Cast<string>()
                .Distinct()
                .ToList();
            var granted = await FetchGrantedAsync(distinctPerms);

            var failures = new List<ValidationFailure>();

            foreach (var item in props)
            {
                //view ✓ +edit ✓ → continue (happy path)
                //view ✗ +edit ✗ → continue (field invisible, silently ignore)
                //view ✓ +edit ✗ → ThrowError(tried to edit without edit permission)
                //view ✗ +edit ✓ → ThrowError(has edit but can't even see the field — suspicious)

                bool hasView = item.ViewPermission == null || granted[item.ViewPermission];
                bool hasEdit = granted[item.EditPermission];

                // Happy path: can both view and edit
                if (hasView && hasEdit)
                    continue;

                // Field completely inaccessible: no view and no edit — silently skip
                if (!hasView && !hasEdit)
                    continue;

                // Any other combination (has edit but no view, or has view but no edit) → violation
                if (item.ThrowError)
                    failures.Add(new ValidationFailure(
                        item.Prop.Name,
                        $"You do not have permission to edit field '{item.Prop.Name}'."));
                else
                    LogFieldPermissionError(request, item);
            }

            if (failures.Any())
                throw new Application.Common.Exceptions.ValidationException(failures);
        }

        private void LogFieldPermissionError(TRequest request, (PropertyInfo Prop, string EditPermission, string? ViewPermission, bool ThrowError) item)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId ?? string.Empty;
            var fieldName = item.Prop.Name;
            _logger.LogError("FieldPermissionError - {RequestName}: User (UserId:{UserId}) does not have permission to edit field '{FieldName}'.",
                requestName, userId, fieldName);
        }

        // ── VIEW ──────────────────────────────────────────────────────────────

        private async Task MaskViewPermissions(TResponse response)
        {
            var directProps = _viewCache.GetOrAdd(typeof(TResponse), GetViewProperties);
            var collections = _collectionCache.GetOrAdd(typeof(TResponse), GetCollectionMeta);

            if (directProps.Count == 0 && collections.Count == 0)
                return;

            // Collect ALL distinct permissions in one batch — direct + every collection item type
            var allPerms = directProps.Select(p => p.Permission)
                .Concat(collections.SelectMany(c => c.ItemProps.Select(p => p.Permission)))
                .Distinct()
                .ToList();

            var granted = await FetchGrantedAsync(allPerms);

            // 1. Mask direct properties on TResponse
            foreach (var item in directProps)
            {
                if (!granted[item.Permission])
                    TrySetNull(response, item.Prop);
            }

            // 2. Recurse into collection properties — mask each item in the list
            foreach (var meta in collections)
            {
                var collectionValue = meta.CollectionProp.GetValue(response) as IEnumerable;
                if (collectionValue == null)
                    continue;

                foreach (var element in collectionValue)
                {
                    if (element == null)
                        continue;

                    foreach (var itemProp in meta.ItemProps)
                    {
                        if (!granted[itemProp.Permission])
                            TrySetNull(element, itemProp.Prop);
                    }
                }
            }
        }

        // ── HELPERS ───────────────────────────────────────────────────────────

        private async Task<Dictionary<string, bool>> FetchGrantedAsync(List<string> permissions)
        {
            var tasks = permissions.Select(p => _currentUserService.HasPermissionAsync(p)).ToList();
            var results = await Task.WhenAll(tasks);

            var granted = new Dictionary<string, bool>(permissions.Count);
            for (int i = 0; i < permissions.Count; i++)
                granted[permissions[i]] = results[i];

            return granted;
        }

        private static void TrySetNull(object target, PropertyInfo prop)
        {
            try { prop.SetValue(target, null); }
            catch { }
        }

        // ── REFLECTION RESOLVERS (run once per type) ──────────────────────────

        private static List<(PropertyInfo Prop, string EditPermission, string? ViewPermission, bool ThrowError)> GetEditProperties(Type type)
        {
            var result = new List<(PropertyInfo Prop, string EditPermission, string? ViewPermission, bool ThrowError)>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = prop.GetCustomAttribute<FieldPermissionAttribute>();
                if (attr != null && attr.EditPermission != null && prop.CanRead)
                    result.Add((prop, attr.EditPermission, attr.ViewPermission, attr.ThrowError));
            }
            return result;
        }

        private static List<(PropertyInfo Prop, string Permission)> GetViewProperties(Type type)
        {
            var result = new List<(PropertyInfo Prop, string Permission)>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = prop.GetCustomAttribute<FieldPermissionAttribute>();
                if (attr != null && attr.ViewPermission != null && prop.CanWrite)
                    result.Add((prop, attr.ViewPermission));
            }
            return result;
        }

        private static List<CollectionMeta> GetCollectionMeta(Type type)
        {
            var result = new List<CollectionMeta>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead)
                    continue;

                var itemType = GetCollectionItemType(prop.PropertyType);
                if (itemType == null || !itemType.IsClass)
                    continue;

                var itemProps = GetViewProperties(itemType);
                if (itemProps.Count > 0)
                    result.Add(new CollectionMeta(prop, itemProps));
            }
            return result;
        }

        private static Type GetCollectionItemType(Type type)
        {
            if (type == typeof(string))
                return null;

            var iface = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                ? type
                : type.GetInterfaces()
                      .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (iface == null)
                return null;

            var args = iface.GetGenericArguments();
            return args.Length > 0 ? args[0] : null;
        }

        // ── INNER CLASS ───────────────────────────────────────────────────────

        private class CollectionMeta
        {
            public PropertyInfo CollectionProp { get; }
            public List<(PropertyInfo Prop, string Permission)> ItemProps { get; }

            public CollectionMeta(PropertyInfo collectionProp, List<(PropertyInfo Prop, string Permission)> itemProps)
            {
                CollectionProp = collectionProp;
                ItemProps = itemProps;
            }
        }
    }
}
