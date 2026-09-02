using System.Text.Json.Serialization;
using Application.Common.Mappings;

namespace Application.PublishEvent.Queries
{
    public class PublishEventDataDto : IMapFrom<Domain.Common.PublishEventData>
    {
        public string Id { get; set; }
        public string CorrelationId { get; set; }
        public string AuditableRequestId { get; set; }
        public string AuditableRequestName { get; set; }
        public string AuditableAssemblyQualifiedName { get; set; }
        public string AuditableSourceEventName { get; set; }
        public string CreatedDateTime { get; set; }
        public string ApiName { get; set; }
        public string CollectionName { get; set; }

        [JsonIgnore]
        //public string EventData { get; set; } // from DB
        public byte[] EventDataBinary { get; set; } // for MsgPack
        public string UserId { get; set; }
        public string OperationType { get; set; }
        public object EventDataJson { get; set; } // deserialized



        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Common.PublishEventData, PublishEventDataDto>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore());
        }
    }
}
