using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieHub.CatalogApi.Dtos;
using MovieHub.CatalogApi.Models;
using MovieHub.CatalogApi.Settings;

namespace MovieHub.CatalogApi.Services;

public class MovieService
{
    private readonly IMongoCollection<Movie> _moviesCollection;

    public MovieService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        MongoDbSettings settings = mongoDbSettings.Value;

        MongoClient mongoClient = new(settings.ConnectionString);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);

        _moviesCollection = mongoDatabase.GetCollection<Movie>(settings.MoviesCollectionName);
    }

    public async Task<PagedResult<Movie>> GetMoviesAsync(int page, int pageSize, string? search)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        FilterDefinition<Movie> filter = Builders<Movie>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(search))
        {
            FilterDefinition<Movie> titleFilter = Builders<Movie>.Filter.Regex(
                movie => movie.Title,
                new BsonRegularExpression(search, "i"));

            FilterDefinition<Movie> directorFilter = Builders<Movie>.Filter.Regex(
                movie => movie.Director,
                new BsonRegularExpression(search, "i"));

            FilterDefinition<Movie> genreFilter = Builders<Movie>.Filter.Regex(
                movie => movie.Genre,
                new BsonRegularExpression(search, "i"));

            filter = Builders<Movie>.Filter.Or(titleFilter, directorFilter, genreFilter);
        }

        long totalCount = await _moviesCollection.CountDocumentsAsync(filter);

        List<Movie> items = await _moviesCollection
            .Find(filter)
            .SortBy(movie => movie.Title)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return new PagedResult<Movie>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = (int)totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<Movie?> GetMovieByIdAsync(string id)
    {
        return await _moviesCollection
            .Find(movie => movie.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Movie> CreateMovieAsync(CreateMovieRequest request)
    {
        Movie movie = new()
        {
            Title = request.Title,
            Director = request.Director,
            Genre = request.Genre,
            Year = request.Year,
            Rating = request.Rating,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _moviesCollection.InsertOneAsync(movie);

        return movie;
    }

    public async Task<Movie?> UpdateMovieAsync(string id, UpdateMovieRequest request)
    {
        Movie? movie = await GetMovieByIdAsync(id);

        if (movie == null)
        {
            return null;
        }

        movie.Title = request.Title;
        movie.Director = request.Director;
        movie.Genre = request.Genre;
        movie.Year = request.Year;
        movie.Rating = request.Rating;
        movie.Description = request.Description;

        await _moviesCollection.ReplaceOneAsync(existingMovie => existingMovie.Id == id, movie);

        return movie;
    }

    public async Task<bool> DeleteMovieAsync(string id)
    {
        DeleteResult result = await _moviesCollection.DeleteOneAsync(movie => movie.Id == id);

        return result.DeletedCount > 0;
    }

    public async Task<List<Movie>> GetTopRatedMoviesAsync()
    {
        return await _moviesCollection
            .Find(Builders<Movie>.Filter.Empty)
            .SortByDescending(movie => movie.Rating)
            .Limit(5)
            .ToListAsync();
    }

    public async Task SeedMoviesAsync()
    {
        long movieCount = await _moviesCollection.CountDocumentsAsync(Builders<Movie>.Filter.Empty);

        if (movieCount > 0)
        {
            return;
        }

        List<Movie> seedMovies = new()
        {
            new Movie
            {
                Title = "The Matrix",
                Director = "The Wachowskis",
                Genre = "Sci-Fi",
                Year = 1999,
                Rating = 9.1,
                Description = "A hacker discovers that reality is a simulation.",
                CreatedAt = DateTime.UtcNow
            },
            new Movie
            {
                Title = "Inception",
                Director = "Christopher Nolan",
                Genre = "Sci-Fi",
                Year = 2010,
                Rating = 8.8,
                Description = "A thief enters people's dreams to steal secrets.",
                CreatedAt = DateTime.UtcNow
            },
            new Movie
            {
                Title = "The Lord of the Rings: The Fellowship of the Ring",
                Director = "Peter Jackson",
                Genre = "Fantasy",
                Year = 2001,
                Rating = 8.9,
                Description = "A young hobbit begins a journey to destroy a powerful ring.",
                CreatedAt = DateTime.UtcNow
            },
            new Movie
            {
                Title = "Interstellar",
                Director = "Christopher Nolan",
                Genre = "Sci-Fi",
                Year = 2014,
                Rating = 8.7,
                Description = "A team travels through a wormhole in search of a new home for humanity.",
                CreatedAt = DateTime.UtcNow
            },
            new Movie
            {
                Title = "The Dark Knight",
                Director = "Christopher Nolan",
                Genre = "Action",
                Year = 2008,
                Rating = 9.0,
                Description = "Batman faces the Joker in Gotham City.",
                CreatedAt = DateTime.UtcNow
            }
        };

        await _moviesCollection.InsertManyAsync(seedMovies);
    }
}