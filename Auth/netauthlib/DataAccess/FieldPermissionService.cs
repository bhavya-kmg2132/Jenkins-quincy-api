using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation.Results;
using NetAuth.Domain.Common;
using NetAuth.Interfaces;

namespace NetAuth.DataAccess
{
    /// <summary>
    /// Enforces field-level permissions by reading <see cref="FieldPermissionAttribute"/>
    /// annotations placed directly on domain entity properties.
    ///
    /// Reflection metadata is cached per entity type so the cost is paid only once.
    /// </summary>
    internal class FieldPermissionService : IFieldPermissionService
    {
        private readonly IIdentityManager _identityManager;

        private static readonly ConcurrentDictionary<Type, List<FieldPermissionDescriptor>> _descriptorCache
            = new ConcurrentDictionary<Type, List<FieldPermissionDescriptor>>();

        public FieldPermissionService(IIdentityManager identityManager)
        {
            _identityManager = identityManager;
        }

        // ── VIEW ──────────────────────────────────────────────────────────────────

        public async Task ApplyViewPermissionsAsync<T>(T entity, string userId) where T : class
        {
            var descriptors = ViewDescriptors(typeof(T));
            if (descriptors.Count == 0) return;

            var granted = await FetchGrantedAsync(descriptors.Select(d => d.ViewPermission!), userId);

            foreach (var d in descriptors)
            {
                if (!granted[d.ViewPermission!])
                    d.Property.SetValue(entity, GetDefault(d.Property.PropertyType));
            }
        }

        // ── CREATE ────────────────────────────────────────────────────────────────

        public async Task ApplyCreatePermissionsAsync<T>(T proposed, string userId) where T : class
        {
            var descriptors = EditDescriptors(typeof(T));
            if (descriptors.Count == 0) return;

            var allPermissions = descriptors
                .SelectMany(d => new[] { d.EditPermission, d.ViewPermission })
                .OfType<string>();

            var granted = await FetchGrantedAsync(allPermissions, userId);

            var failures = new List<ValidationFailure>();

            foreach (var d in descriptors)
            {
                // View gate: if user cannot see the field, clear it and skip edit check.
                if (d.ViewPermission != null && !granted[d.ViewPermission])
                {
                    d.Property.SetValue(proposed, GetDefault(d.Property.PropertyType));
                    continue;
                }

                // Step 2: Edit check — only reached if view passed or no view permission defined
                if (d.EditPermission != null && !granted[d.EditPermission])
                    failures.Add(new ValidationFailure(
                        d.Property.Name,
                        $"You do not have permission to set '{d.Property.Name}'."));
            }

            if (failures.Count > 0)
                throw new FluentValidation.ValidationException(failures);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────────

        public async Task ApplyEditPermissionsAsync<T>(T original, T proposed, string userId) where T : class
        {
            var descriptors = EditDescriptors(typeof(T));
            if (descriptors.Count == 0) return;

            var allPermissions = descriptors
                .SelectMany(d => new[] { d.EditPermission, d.ViewPermission })
                .OfType<string>();

            var granted = await FetchGrantedAsync(allPermissions, userId);

            var failures = new List<ValidationFailure>();

            foreach (var d in descriptors)
            {
                var origValue = d.Property.GetValue(original);
                var newValue = d.Property.GetValue(proposed);

                // View permission gate: user cannot see this field → preserve DB value.
                if (d.ViewPermission != null && !granted[d.ViewPermission])
                {
                    d.Property.SetValue(proposed, origValue);
                    continue;
                }

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
                throw new FluentValidation.ValidationException(failures);
        }

        // ── HELPERS ───────────────────────────────────────────────────────────────

        private async Task<Dictionary<string, bool>> FetchGrantedAsync(IEnumerable<string> permissions, string userId)
        {
            var permList = permissions.Distinct().ToList();
            var tasks = permList.Select(p => _identityManager.AuthHasRequestPermissionAsync(userId, p)).ToList();
            var results = await Task.WhenAll(tasks);

            var granted = new Dictionary<string, bool>(permList.Count, StringComparer.Ordinal);
            for (int i = 0; i < permList.Count; i++)
                granted[permList[i]] = results[i];

            return granted;
        }

        private static object? GetDefault(Type type)
            => type.IsValueType ? Activator.CreateInstance(type) : null;

        // ── REFLECTION CACHE ──────────────────────────────────────────────────────

        private static List<FieldPermissionDescriptor> AllDescriptors(Type type)
        {
            var list = new List<FieldPermissionDescriptor>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || !prop.CanWrite) continue;
                var attr = prop.GetCustomAttribute<FieldPermissionAttribute>();
                if (attr == null || (attr.ViewPermission == null && attr.EditPermission == null)) continue;

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
