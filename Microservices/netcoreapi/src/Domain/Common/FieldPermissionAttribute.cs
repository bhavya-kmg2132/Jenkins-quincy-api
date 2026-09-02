using System;

namespace Domain.Common
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FieldPermissionAttribute : Attribute
    {
        public string ViewPermission { get; }
        public string EditPermission { get; }

        /// <summary>
        /// When true and edit permission is denied on UPDATE, a ValidationException is thrown.
        /// When false, the field is silently reverted to its original DB value instead.
        /// Use false for fields that may be absent from the request (e.g. when a role
        /// has the field hidden on the UI) so the existing DB value is preserved quietly.
        /// </summary>
        public bool ThrowError { get; }

        public FieldPermissionAttribute(string view = null, string edit = null, bool throwError = false)
        {
            ViewPermission = view;
            EditPermission = edit;
            ThrowError = throwError;
        }
    }
}
