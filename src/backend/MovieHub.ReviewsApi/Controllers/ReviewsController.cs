using Microsoft.AspNetCore.Mvc;
using MovieHub.ReviewsApi.Dtos;
using MovieHub.ReviewsApi.Models;
using MovieHub.ReviewsApi.Services;

namespace MovieHub.ReviewsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public ReviewsController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("movie/{movieId}")]
    public async Task<ActionResult<List<Review>>> GetReviewsByMovieId(string movieId)
    {
        List<Review> reviews = await _reviewService.GetReviewsByMovieIdAsync(movieId);

        return Ok(reviews);
    }

    [HttpPost]
    public async Task<ActionResult<Review>> CreateReview(CreateReviewRequest request)
    {
        if (request.Score < 1 || request.Score > 5)
        {
            return BadRequest("Score must be between 1 and 5.");
        }

        Review review = await _reviewService.CreateReviewAsync(request);

        return CreatedAtAction(
            nameof(GetReviewsByMovieId),
            new { movieId = review.MovieId },
            review);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(string id)
    {
        bool deleted = await _reviewService.DeleteReviewAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}