using AspBaseProj.Application.DTOs;
using MediatR;

namespace AspBaseProj.Application.Features.Posts.Commands;

/// <summary>
/// Command to update an existing blog post.
/// </summary>
public sealed record UpdatePostCommand : IRequest<PostDto>
{
    public Guid PostId { get; init; }
    public Guid CurrentUserId { get; init; }
    public bool IsCurrentUserRoot { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ContentHtml { get; init; } = string.Empty;
    public string? Excerpt { get; init; }
    public bool IsPublished { get; init; }
    public List<SocialLinkInputDto> SocialLinks { get; init; } = [];
}

/// <summary>
/// Command to delete a blog post.
/// </summary>
public sealed record DeletePostCommand : IRequest
{
    public Guid PostId { get; init; }
    public Guid CurrentUserId { get; init; }
    public bool IsCurrentUserRoot { get; init; }
}

/// <summary>
/// Command to increment a post's view count.
/// </summary>
public sealed record IncrementViewCountCommand : IRequest
{
    public Guid PostId { get; init; }
}
