using Microsoft.AspNetCore.Mvc;
using MovieHub.CatalogApi.Dtos;
using MovieHub.CatalogApi.Models;

namespace MovieHub.CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private static readonly List<Movie> Movies = new()
    {
        new Movie
        {
            Title = "The Matrix",
            Director = "The Wachowskis",
            Genre = "Sci-Fi",
            Year = 1999,
            Rating = 9.1,
            Description = "A hacker discovers that reality is a simulation."
        },
        new Movie
        {
            Title = "Inception",
            Director = "Christopher Nolan",
            Genre = "Sci-Fi",
            Year = 2010,
            Rating = 8.8,
            Description = "A thief enters people's dreams to steal secrets."
        },
        new Movie
        {
            Title = "The Lord of the Rings: The Fellowship of the Ring",
            Director = "Peter Jackson",
            Genre = "Fantasy",
            Year = 2001,
            Rating = 8.9,
            Description = "A young hobbit begins a journey to destroy a powerful ring."
        }
    };

    [HttpGet]
    public ActionResult<PagedResult<Movie>> GetMovies(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        IEnumerable<Movie> query = Movies;

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(movie =>
                movie.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                movie.Director.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                movie.Genre.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        int totalCount = query.Count();

        List<Movie> items = query
            .OrderBy(movie => movie.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        PagedResult<Movie> result = new()
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<Movie> GetMovieById(string id)
    {
        Movie? movie = Movies.FirstOrDefault(movie => movie.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    [HttpPost]
    public ActionResult<Movie> CreateMovie(CreateMovieRequest request)
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

        Movies.Add(movie);

        return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
    }

    [HttpPut("{id}")]
    public ActionResult<Movie> UpdateMovie(string id, UpdateMovieRequest request)
    {
        Movie? movie = Movies.FirstOrDefault(movie => movie.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        movie.Title = request.Title;
        movie.Director = request.Director;
        movie.Genre = request.Genre;
        movie.Year = request.Year;
        movie.Rating = request.Rating;
        movie.Description = request.Description;

        return Ok(movie);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMovie(string id)
    {
        Movie? movie = Movies.FirstOrDefault(movie => movie.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        Movies.Remove(movie);

        return NoContent();
    }

    [HttpGet("top-rated")]
    public ActionResult<List<Movie>> GetTopRatedMovies()
    {
        List<Movie> movies = Movies
            .OrderByDescending(movie => movie.Rating)
            .Take(5)
            .ToList();

        return Ok(movies);
    }
}