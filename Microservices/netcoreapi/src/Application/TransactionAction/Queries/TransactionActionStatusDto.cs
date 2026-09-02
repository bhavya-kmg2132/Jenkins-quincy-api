using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.TransactionAction.Queries
{
    public class TransactionActionStatusDto
    {
        [JsonPropertyName("transactionCodeName")]
        public string TransactionCodeName { get; set; }

        [JsonPropertyName("PEND")]
        public List<TransactionActionItemDto> Pend { get; set; } = new List<TransactionActionItemDto>();

        [JsonPropertyName("POSTED")]
        public List<TransactionActionItemDto> Post { get; set; } = new List<TransactionActionItemDto>();
    }

    public class TransactionActionItemDto
    {
        [JsonPropertyName("actionName")]
        public string ActionName { get; set; }

        [JsonPropertyName("actionDisplayName")]
        public string ActionDisplayName { get; set; }
    }
}
