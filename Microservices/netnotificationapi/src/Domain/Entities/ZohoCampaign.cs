using System;
using System.Collections.Generic;
using Domain.Common;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.Entities
{
    public class ZohoCampaign : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public string ListKey { get; set; }
        public List<Dictionary<string, string>> ContactInfo { get; set; }
        public List<string> Emails { get; set; }
        public string CompanyName { get; set; }
        public string UserEmailId { get; set; }


        public string ListName { get; set; }
        public string SignupForm { get; set; }   // "public" or "private"
        public string ListDescription { get; set; }


        public string Source { get; set; } = "API"; // default
        public Dictionary<string, string> ContactFields { get; set; } = new();
        public string CampaignName { get; set; }
        public string FromEmail { get; set; }
        public string Subject { get; set; }

        public string CampaignKey { get; set; }
        public string Sort { get; set; } = "desc";  // asc / desc
        public int FromIndex { get; set; } = 0;     // Starting index
        public int Range { get; set; } = 10;        // Number of records
        public string Status { get; set; } = "all"; // all, drafts, scheduled, etc.


        public string ListDetailsEncoded { get; set; }

        // Each entry = one list key + optional segment IDs
        public Dictionary<string, List<string>> ListDetails { get; set; }

        // Optional if you’re serving HTML via ngrok
        public string ContentUrl { get; set; }
        public string TopicId { get; set; }
        public ZohoCampaignReport CampaignReport { get; set; }

        [NonSerialized]
        [JsonIgnore]
        [BsonIgnore]
        private bool _done;

        [JsonIgnore]
        [BsonIgnore]
        public bool Done
        {
            get => _done;
            set
            {
                if (value == true && _done == false)
                {
                    // DomainEvents.Add(new MilestoneCompletedEvent(this));
                }

                _done = value;
            }
        }

        [NonSerialized]
        [JsonIgnore]
        [BsonIgnore]
        private List<DomainEvent> _domainEvents;

        [JsonIgnore]
        [BsonIgnore]
        public List<DomainEvent> DomainEvents
        {
            get
            {
                if (_domainEvents == null)
                {
                    _domainEvents = new List<DomainEvent>();
                }

                return _domainEvents;
            }
            set { _domainEvents = value; }
        }
    }

    public class ZohoCampaignReport
    {

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("campaign-by-loaction")]
        public string CampaignByLocation { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("campaign-reports")]
        public List<CampaignReport> CampaignReports { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("campaign-details")]
        public List<CampaignDetail> CampaignDetails { get; set; }

        [JsonProperty("campaign-reach")]
        public List<CampaignReach> CampaignReach { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
    public class CampaignReport
    {
        [JsonProperty("forward_percent")]
        public string ForwardPercent { get; set; }

        [JsonProperty("hardbounce_count")]
        public string HardbounceCount { get; set; }

        [JsonProperty("unsent_count")]
        public string UnsentCount { get; set; }

        [JsonProperty("bounce_percent")]
        public string BouncePercent { get; set; }

        [JsonProperty("unique_clicked_percent")]
        public string UniqueClickedPercent { get; set; }

        [JsonProperty("unopened")]
        public string Unopened { get; set; }

        [JsonProperty("unsubscribe_percent")]
        public string UnsubscribePercent { get; set; }

        [JsonProperty("spams_count")]
        public string SpamsCount { get; set; }

        [JsonProperty("spam_percent")]
        public string SpamPercent { get; set; }

        [JsonProperty("delivered_percent")]
        public string DeliveredPercent { get; set; }

        [JsonProperty("delivered_count")]
        public string DeliveredCount { get; set; }

        [JsonProperty("complaints_count")]
        public string ComplaintsCount { get; set; }

        [JsonProperty("unopened_percent")]
        public string UnopenedPercent { get; set; }

        [JsonProperty("autoreply_count")]
        public string AutoreplyCount { get; set; }

        [JsonProperty("softbounce_count")]
        public string SoftbounceCount { get; set; }

        [JsonProperty("opens_count")]
        public string OpensCount { get; set; }

        [JsonProperty("campaign_name")]
        public string CampaignName { get; set; }

        [JsonProperty("unique_clicks_count")]
        public string UniqueClicksCount { get; set; }

        [JsonProperty("unsub_count")]
        public string UnsubCount { get; set; }

        [JsonProperty("complaints_percent")]
        public string ComplaintsPercent { get; set; }

        [JsonProperty("unsent_percent")]
        public string UnsentPercent { get; set; }

        [JsonProperty("bounces_count")]
        public string BouncesCount { get; set; }

        [JsonProperty("open_percent")]
        public string OpenPercent { get; set; }

        [JsonProperty("clicksperopenrate")]
        public string ClicksPerOpenRate { get; set; }

        [JsonProperty("forwards_count")]
        public string ForwardsCount { get; set; }

        [JsonProperty("emails_sent_count")]
        public string EmailsSentCount { get; set; }
    }
    public class CampaignDetail
    {
        [JsonProperty("email_type")]
        public string EmailType { get; set; }

        [JsonProperty("campaign_key")]
        public string CampaignKey { get; set; }

        [JsonProperty("reply_to")]
        public string ReplyTo { get; set; }

        [JsonProperty("campaign_name")]
        public string CampaignName { get; set; }

        [JsonProperty("sent_time")]
        public string SentTime { get; set; }

        [JsonProperty("email_subject")]
        public string EmailSubject { get; set; }

        [JsonProperty("email_options")]
        public string EmailOptions { get; set; }

        [JsonProperty("created_time")]
        public string CreatedTime { get; set; }

        [JsonProperty("email_from")]
        public string EmailFrom { get; set; }

    }
    public class CampaignReach
    {
        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("twitter")]
        public string Twitter { get; set; }

        [JsonProperty("other")]
        public string Other { get; set; }

        [JsonProperty("linkedin")]
        public string Linkedin { get; set; }

        [JsonProperty("facebook")]
        public string Facebook { get; set; }

        [JsonProperty("emails")]
        public string Emails { get; set; }
    }


}
