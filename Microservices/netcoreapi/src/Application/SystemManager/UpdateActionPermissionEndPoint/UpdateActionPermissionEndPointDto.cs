namespace Application.SystemManager.UpdateActionPermissionEndPoint
{
    /// <summary>
    /// One MediatR request/query discovered by reflecting over controller actions, paired with its route.
    /// </summary>
    public class UpdateActionPermissionEndPointDto
    {
        public string PermissionValue { get; set; }
        public string PermissionDisplayName { get; set; }
        public string ActionPermissionEndPoint { get; set; }
    }
}
