namespace NetAuth.Contract.DataContract.Requests
{
    public class AddPermissionGrantedForUserRequest
    {
        public List<string> PermissionIds { get; set; }
        public string UserId { get; set; }

    }
}
