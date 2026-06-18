using AspBaseProj.Application.Features.Media;
using AspBaseProj.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspBaseProj.Presentation.Endpoints;

/// <summary>
/// Media API endpoints for serving and uploading images.
/// </summary>
public static class MediaEndpoints
{
    public static IEndpointRouteBuilder MapMediaEndpoints(this IEndpointRouteBuilder app)
    {
        // Serve media by ID (public - images are embedded in posts)
        app.MapGet("/api/media/{mediaId:guid}", async (Guid mediaId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetMediaDataQuery { MediaId = mediaId });

            if (result is null)
                return Results.NotFound();

            var (data, contentType, fileName) = result.Value;
            return Results.File(data, contentType, fileName);
        })
        .AllowAnonymous()
        .WithSummary("Get media file by ID")
        .WithTags("Media");

        // Upload media (authenticated)
        app.MapPost("/api/media/upload", async (
            HttpRequest request,
            IMediator mediator,
            UserManager<AppUser> userManager,
            HttpContext httpContext) =>
        {
            var user = await userManager.GetUserAsync(httpContext.User);
            if (user is null)
                return Results.Unauthorized();

            if (!request.HasFormContentType)
                return Results.BadRequest("Expected form data.");

            var form = await request.ReadFormAsync();
            var file = form.Files.FirstOrDefault();
            if (file is null || file.Length == 0)
                return Results.BadRequest("No file uploaded.");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var result = await mediator.Send(new UploadMediaCommand
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Data = ms.ToArray(),
                UploadedById = user.Id,
                PostId = form.TryGetValue("postId", out var postIdStr) && Guid.TryParse(postIdStr, out var pid) ? pid : null
            });

            return Results.Ok(new { mediaId = result.Id, url = $"/api/media/{result.Id}" });
        })
        .RequireAuthorization()
        .WithSummary("Upload a media file")
        .WithTags("Media");

        return app;
    }
}
