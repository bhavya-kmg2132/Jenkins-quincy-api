namespace NetAuth.Domain.Entities.CoreRequests
{
    internal class ActivateOrInActivateUser
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
