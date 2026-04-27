using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://localhost:8080")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Frontend");

app.MapGet("/health", () => Results.Ok(new
{
    service = "MovieHub.McpServer",
    status = "Healthy",
    timestamp = DateTime.UtcNow
}));

app.MapPost("/mcp", async (HttpRequest request) =>
{
    JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    McpRequest? mcpRequest = await JsonSerializer.DeserializeAsync<McpRequest>(
        request.Body,
        jsonOptions);

    if (mcpRequest is null)
    {
        return Results.BadRequest(new
        {
            error = "Invalid MCP request body."
        });
    }

    object response = mcpRequest.Method switch
    {
        "initialize" => CreateInitializeResponse(mcpRequest.Id),
        "tools/list" => CreateToolsListResponse(mcpRequest.Id),
        "tools/call" => CreateToolCallResponse(mcpRequest),
        _ => CreateMethodNotFoundResponse(mcpRequest.Id, mcpRequest.Method)
    };

    return Results.Ok(response);
});

app.Run();

static McpResponse CreateInitializeResponse(JsonElement? id)
{
    return new McpResponse
    {
        Id = id,
        Result = new
        {
            protocolVersion = "2025-11-25",
            capabilities = new
            {
                tools = new { }
            },
            serverInfo = new
            {
                name = "moviehub-mcp-server",
                version = "1.0.0"
            }
        }
    };
}

static McpResponse CreateToolsListResponse(JsonElement? id)
{
    return new McpResponse
    {
        Id = id,
        Result = new
        {
            tools = new object[]
            {
                new
                {
                    name = "get_project_info",
                    description = "Returns general information about the MovieHub E2E DevOps project.",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new { }
                    }
                },
                new
                {
                    name = "get_architecture_summary",
                    description = "Returns a short architecture summary of the MovieHub system.",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new { }
                    }
                },
                new
                {
                    name = "get_deployment_checklist",
                    description = "Returns a checklist for Docker, Kubernetes and ArgoCD deployment.",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new { }
                    }
                }
            }
        }
    };
}

static object CreateToolCallResponse(McpRequest request)
{
    string? toolName = null;

    if (request.Params.HasValue &&
        request.Params.Value.TryGetProperty("name", out JsonElement nameElement))
    {
        toolName = nameElement.GetString();
    }

    string text = toolName switch
    {
        "get_project_info" => """
            MovieHub is an end-to-end DevOps demo project.
            It contains an Angular frontend, ASP.NET backend services, MongoDB, Docker, GitHub Actions, Kubernetes and ArgoCD.
            """,

        "get_architecture_summary" => """
            MovieHub architecture:
            - Angular frontend served by Nginx
            - Catalog API for movie data
            - Reviews API for movie reviews
            - MongoDB for persistence
            - MCP Server as an independent backend component
            - Docker Compose for local orchestration
            - Kubernetes manifests for deployment
            - ArgoCD for GitOps CD
            """,

        "get_deployment_checklist" => """
            Deployment checklist:
            1. Build and push Docker images with GitHub Actions.
            2. Verify GHCR packages.
            3. Apply Kubernetes manifests or let ArgoCD sync them.
            4. Check pods in the moviehub namespace.
            5. Port-forward frontend, Catalog API and Reviews API.
            6. Test movie list, details and reviews.
            """,

        _ => $"Unknown tool: {toolName}"
    };

    return new McpResponse
    {
        Id = request.Id,
        Result = new
        {
            content = new object[]
            {
                new
                {
                    type = "text",
                    text
                }
            }
        }
    };
}

static McpErrorResponse CreateMethodNotFoundResponse(JsonElement? id, string method)
{
    return new McpErrorResponse
    {
        Id = id,
        Error = new
        {
            code = -32601,
            message = $"Method not found: {method}"
        }
    };
}

public sealed class McpRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public JsonElement? Id { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("params")]
    public JsonElement? Params { get; set; }
}

public sealed class McpResponse
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public JsonElement? Id { get; set; }

    [JsonPropertyName("result")]
    public object? Result { get; set; }
}

public sealed class McpErrorResponse
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public JsonElement? Id { get; set; }

    [JsonPropertyName("error")]
    public object? Error { get; set; }
}