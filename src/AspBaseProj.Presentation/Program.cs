using AspBaseProj.Presentation.Extensions;
using AspBaseProj.Presentation.Middleware;
using AspBaseProj.Presentation.Options;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// --- Configuration & Options ---
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<BlogOptions>(builder.Configuration.GetSection("Blog"));

// --- Global Exception Handler (Problem Details RFC 7807) ---
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- Application Layer (CQRS, Validation) ---
builder.Services.AddApplicationServices();

// --- Infrastructure Layer (EF Core, Identity, Services) ---
builder.Services.AddInfrastructureServices(builder.Configuration);

// --- Web: Razor Pages + MVC ---
builder.Services.AddRazorPages(options =>
{
    // Add additional routes for Posts pages
    options.Conventions.AddPageRoute("/Posts/Detail", "/posts/{slug}");
    options.Conventions.AddPageRoute("/Posts/Edit", "/posts/create");
    options.Conventions.AddPageRoute("/Posts/Edit", "/posts/edit/{slug}");
});
builder.Services.AddControllers();

// --- API Versioning ---
builder.Services.AddApiVersioningConfiguration();

// --- Authentication & Authorization (Cookie + JWT) ---
builder.Services.AddWebAuthentication(builder.Configuration);
builder.Services.AddWebAuthorization();

// --- OpenAPI / Swagger ---
builder.Services.AddOpenApiDocumentation();

// --- Cross-cutting: Compression, Rate Limiting, CORS ---
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
        policy.WithOrigins("https://localhost:5001")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// --- Rate Limiting ---
builder.Services.AddRateLimiter(options =>
{
    // Global policy: 100 requests per minute per user/IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // Auth endpoint: stricter limit (10 per minute)
    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

var app = builder.Build();

// --- Middleware Pipeline (order matters) ---
// ExceptionHandling -> HSTS -> HTTPS Redirection -> RateLimiting -> Routing -> CORS -> Auth -> Endpoints
app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCompression();

// CSP Headers for web output
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
        "img-src 'self' data: blob:; " +
        "font-src 'self' https://cdn.jsdelivr.net; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none';");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.UseStaticFiles();
app.UseRateLimiter();

app.UseRouting();

app.UseCors("ApiPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Swagger UI (dev only)
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.MapRazorPages();
app.MapControllers();

// API endpoints (registered via extension)
app.MapApiEndpoints();

// Ensure database is created and seeded
await app.Services.EnsureDatabaseCreatedAsync();

app.Run();
