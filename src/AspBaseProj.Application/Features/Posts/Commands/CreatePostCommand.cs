using AspBaseProj.Application.DTOs;
using MediatR;

namespace AspBaseProj.Application.Features.Posts.Commands;

/// <summary>
/// Command to create a new blog post.
/// </summary>
public sealed record CreatePostCommand : IRequest<PostDto>
{
    public Guid AuthorId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ContentHtml { get; init; } = string.Empty;
    public string? Excerpt { get; init; }
    public bool IsPublished { get; init; }
    public List<SocialLinkInputDto> SocialLinks { get; init; } = [];
}
