using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class ZohoAuthentication
    {

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id { get; set; }
        public string CompanyId { get; set; }   // FK to Companies
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiryTime { get; set; } // Store wh en token expires
        public string Scope { get; set; }
        public string ApiDomain { get; set; }
        public string TokenType { get; set; }

    }
    public class ZohoAccessTokenResponse
    {
        public bool AuthRequired { get; set; }
        public string AccessToken { get; set; }
        public string Message { get; set; }
    }
}

