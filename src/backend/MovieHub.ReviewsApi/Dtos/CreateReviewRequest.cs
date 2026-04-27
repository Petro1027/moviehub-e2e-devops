namespace MovieHub.ReviewsApi.Dtos;

public class CreateReviewRequest
{
    public string MovieId { get; set; } = string.Empty;

    public string ReviewerName { get; set; } = string.Empty;

    public int Score { get; set; }

    public string Comment { get; set; } = string.Empty;
}