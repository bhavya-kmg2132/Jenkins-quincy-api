namespace NetAuth.Contract.DataContract.Entities
{
    /// <summary>
    /// Used for ApplicationRequest layer mapping through RoleUiPermissionVM.
    /// </summary>
    public class RoleUiPermission
    {
        public string RoleId { get; set; }
        public string UiPermissionId { get; set; }
    }
}
