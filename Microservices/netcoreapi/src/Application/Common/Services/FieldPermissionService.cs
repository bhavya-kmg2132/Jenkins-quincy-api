using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Common;
using FluentValidation.Results;
using AppValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Common.Services
{
    /// <summary>
    /// Enforces field-level permissions by reading <see cref="FieldPermissionAttribute"/>
    /// annotations placed directly on domain entity properties.
    ///
    /// Reflection metadata is cached per entity type so the cost is paid only once.
    /// </summary>
    public class FieldPermissionService : IFieldPermissionService
    {
        private readonly ICurrentUserService _currentUser;

        private static readonly ConcurrentDictionary<Type, List<FieldPermissionDescriptor>> _descriptorCache
            = new ConcurrentDictionary<Type, List<FieldPermissionDescriptor>>();

        public FieldPermissionService(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        // ── VIEW ──────────────────────────────────────────────────────────────────

        public async Task ApplyViewPermissionsAsync<T>(T entity) where T : class
        {
            var descriptors = ViewDescriptors(typeof(T));
            if (descriptors.Count == 0) return;

            var granted = await FetchGrantedAsync(descriptors.Select(d => d.ViewPermission!));

            foreach (var d in descriptors)
            {
                if (!granted[d.ViewPermission!])
                    d.Property.SetValue(entity, GetDefault(d.Property.PropertyType));
            }
        }

        // ── UPDATE ────────────────────────────────────────────────────────────────

        public async Task ApplyEditPermissionsAsync<T>(T original, T proposed) where T : class
        {
            var descriptors = EditDescriptors(typeof(T));
            if (descriptors.Count == 0) return;

            // Fetch both view and edit permissions in one round-trip.
            // View permission is used as a gate: if the user cannot see the field,
            // the DB value is preserved regardless of what the request sent.
            var allPermissions = descriptors
                .SelectMany(d => new[] { d.EditPermission, d.ViewPermission })
                .Where(p => p != null)
                .Cast<string>();

            var granted = await FetchGrantedAsync(allPermissions);

            var failures = new List<ValidationFailure>();

            foreach (var d in descriptors)
            {
                var origValue = d.Property.GetValue(original);
                var newValue = d.Property.GetValue(proposed);

                // View permission gate: user cannot see this field → preserve DB value
                // regardless of what was sent (absent, null, or any value).
                if (d.ViewPermission != null && !granted[d.ViewPermission])
                {
                    d.Property.SetValue(proposed, origValue);
                    continue;
                }

                // User can view the field → proceed with edit comparison.
                if (Equals(origValue, newValue)) continue;

                if (!granted[d.EditPermission!])
                {
                    if (d.ThrowError)
                        failures.Add(new ValidationFailure(
                            d.Property.Name,
                            $"You do not have permission to edit '{d.Property.Name}'."));
                    else
                        d.Property.SetValue(proposed, origValue);
                }
            }

            if (failures.Count > 0)
                throw new AppValidationException(failures);
        }

        // ── HELPERS ───────────────────────────────────────────────────────────────

        private async Task<Dictionary<string, bool>> FetchGrantedAsync(IEnumerable<string> permissions)
        {
            var permList = permissions.Distinct().ToList();
            var tasks = permList.Select(p => _currentUser.HasPermissionAsync(p)).ToList();
            var results = await Task.WhenAll(tasks);

            var granted = new Dictionary<string, bool>(permList.Count, StringComparer.Ordinal);
            for (int i = 0; i < permList.Count; i++)
                granted[permList[i]] = results[i];

            return granted;
        }

        private static object? GetDefault(Type type)
            => type.IsValueType ? Activator.CreateInstance(type) : null;

        private static bool IsDefault(object? value, Type type)
            => value == null || Equals(value, GetDefault(type));

        // ── REFLECTION CACHE ──────────────────────────────────────────────────────

        private static List<FieldPermissionDescriptor> AllDescriptors(Type type)
        {
            var list = new List<FieldPermissionDescriptor>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || !prop.CanWrite) continue;
                var attr = prop.GetCustomAttribute<FieldPermissionAttribute>();
                if (attr != null)
                    list.Add(new FieldPermissionDescriptor(prop, attr.ViewPermission, attr.EditPermission, attr.ThrowError));
            }
            return list;
        }

        private static List<FieldPermissionDescriptor> ViewDescriptors(Type type)
            => _descriptorCache
               .GetOrAdd(type, AllDescriptors)
               .Where(d => d.ViewPermission != null)
               .ToList();

        private static List<FieldPermissionDescriptor> EditDescriptors(Type type)
            => _descriptorCache
               .GetOrAdd(type, AllDescriptors)
               .Where(d => d.EditPermission != null)
               .ToList();

        // ── INNER TYPE ────────────────────────────────────────────────────────────

        private sealed record FieldPermissionDescriptor(
            PropertyInfo Property,
            string? ViewPermission,
            string? EditPermission,
            bool ThrowError);
    }
}
