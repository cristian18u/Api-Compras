using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiUser.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    internal string? userId { get; set; }
    public string nameUser { get; set; } = null!;
    public string name { get; set; } = null!;
    public string documentId { get; set; } = null!;
    public string countNumber { get; set; } = null!;
    public int spaceAvailable { get; set; }
    public string[] movements { get; set; } = null!;
}
