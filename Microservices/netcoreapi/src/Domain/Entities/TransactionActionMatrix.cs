namespace Domain.Entities
{
    public class TransactionActionMatrix
    {
        public string Id { get; set; }
        public int TransactionCode { get; set; }
        public string TransactionCodeName { get; set; }
        public string Status { get; set; }
        public string ActionName { get; set; }
        public string ActionDisplayName { get; set; }
        public int DisplayOrder { get; set; }
    }
}
