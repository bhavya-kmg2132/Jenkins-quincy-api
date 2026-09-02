namespace NetAuth.Domain.Common
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal class FieldPermissionAttribute : Attribute
    {
        public string? ViewPermission { get; }
        public string? EditPermission { get; }

        /// <summary>
        /// When true and edit permission is denied on UPDATE, a ValidationException is thrown.
        /// When false, the field is silently reverted to its original DB value instead.
        /// </summary>
        public bool ThrowError { get; }

        public FieldPermissionAttribute(string? view = null, string? edit = null, bool throwValidationMessage = false)
        {
            ViewPermission = view;
            EditPermission = edit;
            ThrowError = throwValidationMessage;
        }
    }
}
