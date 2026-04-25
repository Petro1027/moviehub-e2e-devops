namespace MovieHub.CatalogApi.Models;

public class Movie
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Title { get; set; } = string.Empty;

    public string Director { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public int Year { get; set; }

    public double Rating { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}