namespace NetAuth.Contract.DataContract.Requests
{
    public class ActivateOrInActivateUser
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
