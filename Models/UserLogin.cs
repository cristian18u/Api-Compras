using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiUser.Models;

public class UserLogin
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    internal string? id { get; set; }
    public string user { get; set; } = null!;
    public string password { get; set; } = null!;
}