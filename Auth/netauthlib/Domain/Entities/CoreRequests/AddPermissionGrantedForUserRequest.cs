namespace NetAuth.Domain.Entities.CoreRequests
{
    internal class AddPermissionGrantedForUserRequest
    {
        public List<string> PermissionIds { get; set; }
        public string UserId { get; set; }

    }
}
