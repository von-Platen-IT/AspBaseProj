using AspBaseProj.Application.DTOs;
using MediatR;

namespace AspBaseProj.Application.Features.Posts.Queries;

/// <summary>
/// Query to get a paginated list of published posts.
/// </summary>
public sealed record GetPostListQuery : IRequest<PagedResult<PostSummaryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
}

/// <summary>
/// Query to get a single post by slug (with social links).
/// </summary>
public sealed record GetPostBySlugQuery : IRequest<PostDto>
{
    public string Slug { get; init; } = string.Empty;
}

/// <summary>
/// Query to get posts by a specific author.
/// </summary>
public sealed record GetPostsByAuthorQuery : IRequest<List<PostSummaryDto>>
{
    public Guid AuthorId { get; init; }
    public bool IncludeUnpublished { get; init; }
}

/// <summary>
/// Query to get all posts (admin content management).
/// </summary>
public sealed record GetAllPostsQuery : IRequest<List<PostSummaryDto>>
{
    public bool IncludeUnpublished { get; init; } = true;
}
