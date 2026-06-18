using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Features.Comments.Commands;
using AspBaseProj.Application.Features.Comments.Queries;
using AspBaseProj.Application.Features.Posts.Commands;
using AspBaseProj.Application.Features.Posts.Queries;
using AspBaseProj.Domain.Entities;
using AspBaseProj.Presentation.Authorization;
using AspBaseProj.Presentation.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspBaseProj.Presentation.Endpoints;

/// <summary>
/// Web API endpoints mirroring UI functionality for Author and Viewer groups.
/// Uses JWT bearer authentication.
/// </summary>
public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpointsV1(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}");

        // --- Auth ---
        MapAuthEndpoints(group);

        // --- Posts ---
        MapPostEndpoints(group);

        // --- Comments ---
        MapCommentEndpoints(group);

        return app;
    }

    private static void MapAuthEndpoints(IEndpointRouteBuilder group)
    {
        group.MapPost("/auth/token", async (
            [FromBody] LoginRequest request,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJwtTokenService tokenService) =>
        {
            var user = await userManager.FindByNameAsync(request.UserNameOrEmail)
                       ?? await userManager.FindByEmailAsync(request.UserNameOrEmail);

            if (user is null || !user.IsActive)
                return Results.Unauthorized();

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return Results.Unauthorized();

            var token = tokenService.GenerateToken(user.Id, user.UserName!, user.Email!, user.Group.ToString(), user.IsRoot);

            return Results.Ok(new { token, userName = user.UserName, group = user.Group.ToString(), isRoot = user.IsRoot });
        })
        .AllowAnonymous()
        .WithSummary("Get JWT token for API authentication")
        .WithTags("Auth");
    }

    private static void MapPostEndpoints(IEndpointRouteBuilder group)
    {
        // Get post list (Viewer+)
        group.MapGet("/posts", async (
            IMediator mediator,
            [AsParameters] PostListRequest request) =>
        {
            var result = await mediator.Send(new GetPostListQuery
            {
                PageNumber = request.PageNumber ?? 1,
                PageSize = request.PageSize ?? 10,
                SearchTerm = request.Search
            });
            return Results.Ok(result);
        })
        .AllowAnonymous()
        .WithSummary("Get paginated list of published posts")
        .WithTags("Posts");

        // Get post by slug (Viewer+)
        group.MapGet("/posts/{slug}", async (string slug, IMediator mediator) =>
        {
            try
            {
                var post = await mediator.Send(new GetPostBySlugQuery { Slug = slug });
                return Results.Ok(post);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .AllowAnonymous()
        .WithSummary("Get a single post by slug")
        .WithTags("Posts");

        // Create post (Author+)
        group.MapPost("/posts", async (
            [FromBody] PostInputDto input,
            IMediator mediator,
            UserManager<AppUser> userManager,
            HttpContext httpContext) =>
        {
            var user = await userManager.GetUserAsync(httpContext.User);
            if (user is null) return Results.Unauthorized();

            var post = await mediator.Send(new CreatePostCommand
            {
                AuthorId = user.Id,
                Title = input.Title,
                ContentHtml = input.ContentHtml,
                Excerpt = input.Excerpt,
                IsPublished = input.IsPublished,
                SocialLinks = input.SocialLinks
            });
            return Results.Created($"/api/v1/posts/{post.Slug}", post);
        })
        .RequireAuthorization(Policies.AuthorOrAdmin)
        .WithSummary("Create a new blog post")
        .WithTags("Posts");

        // Update post (Author+)
        group.MapPut("/posts/{slug}", async (
            string slug,
            [FromBody] PostInputDto input,
            IMediator mediator,
            UserManager<AppUser> userManager,
            HttpContext httpContext) =>
        {
            var user = await userManager.GetUserAsync(httpContext.User);
            if (user is null) return Results.Unauthorized();

            try
            {
                var existing = await mediator.Send(new GetPostBySlugQuery { Slug = slug });
                var post = await mediator.Send(new UpdatePostCommand
                {
                    PostId = existing.Id,
                    CurrentUserId = user.Id,
                    IsCurrentUserRoot = user.IsRoot,
                    Title = input.Title,
                    ContentHtml = input.ContentHtml,
                    Excerpt = input.Excerpt,
                    IsPublished = input.IsPublished,
                    SocialLinks = input.SocialLinks
                });
                return Results.Ok(post);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
        })
        .RequireAuthorization(Policies.AuthorOrAdmin)
        .WithSummary("Update an existing blog post")
        .WithTags("Posts");

        // Delete post (Author+)
        group.MapDelete("/posts/{slug}", async (
            string slug,
            IMediator mediator,
            UserManager<AppUser> userManager,
            HttpContext httpContext) =>
        {
            var user = await userManager.GetUserAsync(httpContext.User);
            if (user is null) return Results.Unauthorized();

            try
            {
                var existing = await mediator.Send(new GetPostBySlugQuery { Slug = slug });
                await mediator.Send(new DeletePostCommand
                {
                    PostId = existing.Id,
                    CurrentUserId = user.Id,
                    IsCurrentUserRoot = user.IsRoot
                });
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
        })
        .RequireAuthorization(Policies.AuthorOrAdmin)
        .WithSummary("Delete a blog post")
        .WithTags("Posts");
    }

    private static void MapCommentEndpoints(IEndpointRouteBuilder group)
    {
        // Get comments for a post (Viewer+)
        group.MapGet("/posts/{slug}/comments", async (string slug, IMediator mediator) =>
        {
            try
            {
                var post = await mediator.Send(new GetPostBySlugQuery { Slug = slug });
                var comments = await mediator.Send(new GetCommentsByPostQuery { PostId = post.Id });
                return Results.Ok(comments);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .AllowAnonymous()
        .WithSummary("Get comments for a post")
        .WithTags("Comments");

        // Add comment (Viewer+ or guest)
        group.MapPost("/posts/{slug}/comments", async (
            string slug,
            [FromBody] CommentInputDto input,
            IMediator mediator,
            UserManager<AppUser> userManager,
            HttpContext httpContext) =>
        {
            try
            {
                var post = await mediator.Send(new GetPostBySlugQuery { Slug = slug });

                Guid? userId = null;
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    var user = await userManager.GetUserAsync(httpContext.User);
                    userId = user?.Id;
                }

                var comment = await mediator.Send(new AddCommentCommand
                {
                    PostId = post.Id,
                    ParentCommentId = input.ParentCommentId,
                    UserId = userId,
                    GuestName = input.GuestName,
                    GuestEmail = input.GuestEmail,
                    Content = input.Content
                });

                return Results.Created($"/api/v1/comments/{comment.Id}", comment);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .AllowAnonymous()
        .WithSummary("Add a comment to a post")
        .WithTags("Comments");
    }
}

// --- Request/Response DTOs for API ---

public sealed record LoginRequest
{
    public string UserNameOrEmail { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public sealed record PostListRequest
{
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
    public string? Search { get; init; }
}
