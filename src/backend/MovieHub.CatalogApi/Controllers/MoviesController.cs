using Microsoft.AspNetCore.Mvc;
using MovieHub.CatalogApi.Dtos;
using MovieHub.CatalogApi.Models;
using MovieHub.CatalogApi.Services;

namespace MovieHub.CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly MovieService _movieService;

    public MoviesController(MovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<Movie>>> GetMovies(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        PagedResult<Movie> result = await _movieService.GetMoviesAsync(page, pageSize, search);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Movie>> GetMovieById(string id)
    {
        Movie? movie = await _movieService.GetMovieByIdAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    [HttpPost]
    public async Task<ActionResult<Movie>> CreateMovie(CreateMovieRequest request)
    {
        Movie movie = await _movieService.CreateMovieAsync(request);

        return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Movie>> UpdateMovie(string id, UpdateMovieRequest request)
    {
        Movie? movie = await _movieService.UpdateMovieAsync(id, request);

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(string id)
    {
        bool deleted = await _movieService.DeleteMovieAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("top-rated")]
    public async Task<ActionResult<List<Movie>>> GetTopRatedMovies()
    {
        List<Movie> movies = await _movieService.GetTopRatedMoviesAsync();

        return Ok(movies);
    }
}