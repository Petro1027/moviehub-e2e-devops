namespace MovieHub.CatalogApi.Dtos;

public class UpdateMovieRequest
{
    public string Title { get; set; } = string.Empty;

    public string Director { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public int Year { get; set; }

    public double Rating { get; set; }

    public string Description { get; set; } = string.Empty;
}