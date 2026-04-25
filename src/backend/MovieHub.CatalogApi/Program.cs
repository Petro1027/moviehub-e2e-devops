using MovieHub.CatalogApi.Services;
using MovieHub.CatalogApi.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton<MovieService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

MovieService movieService = app.Services.GetRequiredService<MovieService>();
await movieService.SeedMoviesAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();