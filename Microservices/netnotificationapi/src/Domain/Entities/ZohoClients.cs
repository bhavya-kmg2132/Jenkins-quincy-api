using MongoDB.Bson.Serialization.Attributes;

public class ZohoClients
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }          // Internal CompanyId
    public string Name { get; set; }        // Company Name
    public string[] Domain { get; set; }        // Company Domain

}


