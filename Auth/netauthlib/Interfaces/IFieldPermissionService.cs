namespace NetAuth.Interfaces
{
    /// <summary>
    /// Enforces field-level permissions declared via <see cref="NetAuth.Domain.Common.FieldPermissionAttribute"/>
    /// on domain entity properties.
    /// </summary>
    internal interface IFieldPermissionService
    {
        /// <summary>Nulls out properties the user cannot see. Call before mapping entity to a DTO.</summary>
        Task ApplyViewPermissionsAsync<T>(T entity, string userId) where T : class;

        /// <summary>For CREATE: throws if user set a protected field without the required permission.</summary>
        Task ApplyCreatePermissionsAsync<T>(T proposed, string userId) where T : class;

        /// <summary>
        /// For UPDATE: reverts or throws for each field the user changed without edit permission.
        /// ThrowValidationMessage=false silently reverts; =true throws ValidationException.
        /// </summary>
        Task ApplyEditPermissionsAsync<T>(T original, T proposed, string userId) where T : class;
    }
}
