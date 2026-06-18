using AspBaseProj.Infrastructure.Data;
using AspBaseProj.Infrastructure.Services;
using AspBaseProj.Presentation.Endpoints;
using AspBaseProj.Presentation.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AspBaseProj.Presentation.Extensions;

/// <summary>
/// Extension methods for configuring the web application pipeline.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures native .NET OpenAPI endpoint (development only).
    /// Serves the OpenAPI document at /openapi/v1.json
    /// </summary>
    public static IApplicationBuilder UseSwaggerUI(this WebApplication app)
    {
        // .NET 10 native OpenAPI: document is available at /openapi/v1.json
        // A simple Swagger-like UI can be added later or use Scalar/SwaggerUI separately
        app.MapOpenApi();
        return app;
    }

    /// <summary>
    /// Maps API endpoints (Minimal API). Placeholder for Phase 7.
    /// </summary>
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var versionedApi = app.MapGroup("/api/v{version:apiVersion}").WithApiVersionSet(apiVersionSet);

        versionedApi.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
            .AllowAnonymous()
            .WithSummary("Health check endpoint");

        // API v1 endpoints (Auth, Posts, Comments)
        app.MapApiEndpointsV1();

        // Media endpoints (not versioned - used by both web and API)
        app.MapMediaEndpoints();

        return app;
    }

    /// <summary>
    /// Ensures the database is created and seeded with initial data.
    /// </summary>
    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var context = sp.GetRequiredService<ApplicationDbContext>();
        var seeder = sp.GetRequiredService<IDatabaseSeeder>();
        var blogOptions = sp.GetRequiredService<IOptions<BlogOptions>>().Value;

        await context.Database.MigrateAsync();
        await seeder.SeedAsync(blogOptions.RootUserName, blogOptions.RootEmail, blogOptions.RootPassword);
    }
}
