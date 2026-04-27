using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieHub.ReviewsApi.Models;

public class Review
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string MovieId { get; set; } = string.Empty;

    public string ReviewerName { get; set; } = string.Empty;

    public int Score { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}