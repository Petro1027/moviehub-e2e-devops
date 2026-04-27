using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MovieHub.ReviewsApi.Dtos;
using MovieHub.ReviewsApi.Models;
using MovieHub.ReviewsApi.Settings;

namespace MovieHub.ReviewsApi.Services;

public class ReviewService
{
    private readonly IMongoCollection<Review> _reviewsCollection;

    public ReviewService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        MongoDbSettings settings = mongoDbSettings.Value;

        MongoClient mongoClient = new(settings.ConnectionString);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);

        _reviewsCollection = mongoDatabase.GetCollection<Review>(settings.ReviewsCollectionName);
    }

    public async Task<List<Review>> GetReviewsByMovieIdAsync(string movieId)
    {
        return await _reviewsCollection
            .Find(review => review.MovieId == movieId)
            .SortByDescending(review => review.CreatedAt)
            .ToListAsync();
    }

    public async Task<Review> CreateReviewAsync(CreateReviewRequest request)
    {
        Review review = new()
        {
            MovieId = request.MovieId,
            ReviewerName = request.ReviewerName,
            Score = request.Score,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewsCollection.InsertOneAsync(review);

        return review;
    }

    public async Task<bool> DeleteReviewAsync(string id)
    {
        DeleteResult result = await _reviewsCollection.DeleteOneAsync(review => review.Id == id);

        return result.DeletedCount > 0;
    }
}