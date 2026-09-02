using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// Enforces field-level permissions declared via <see cref="Domain.Common.FieldPermissionAttribute"/>
    /// on domain entity properties.
    ///
    /// Three entry points map to the three handler types:
    ///   - ApplyViewPermissionsAsync  — called after fetching an entity; masks fields the user cannot see.
    ///   - ApplyCreatePermissionsAsync — called after building a new entity; throws if the user set a
    ///                                   protected field without the required permission.
    ///   - ApplyEditPermissionsAsync  — called after mapping request → entity; reverts or throws for
    ///                                   fields the user changed without edit permission.
    /// </summary>
    public interface IFieldPermissionService
    {
        /// <summary>
        /// Nulls out (or zero-fills for value types) any entity property annotated with
        /// <c>view:</c> that the current user is not permitted to see.
        /// Call this BEFORE mapping the entity to a response DTO.
        /// </summary>
        Task ApplyViewPermissionsAsync<T>(T entity) where T : class;

        /// <summary>
        /// For UPDATE: compares <paramref name="original"/> (DB state) against
        /// <paramref name="proposed"/> (handler-built state). For each field protected
        /// by an edit permission:
        ///   - If the value changed and the user lacks permission:
        ///       ThrowValidationMessage=true  → accumulates a validation failure and throws.
        ///       ThrowValidationMessage=false → reverts to the original value (DB value preserved).
        /// Call this AFTER building the updated entity, BEFORE persisting.
        /// </summary>
        Task ApplyEditPermissionsAsync<T>(T original, T proposed) where T : class;
    }
}
