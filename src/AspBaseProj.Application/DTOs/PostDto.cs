namespace AspBaseProj.Application.DTOs;

/// <summary>
/// Data transfer object for a blog post (list/detail views).
/// </summary>
public sealed record PostDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string ContentHtml { get; init; } = string.Empty;
    public string? Excerpt { get; init; }
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public bool IsPublished { get; init; }
    public DateTime? PublishedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public int ViewCount { get; init; }
    public List<SocialLinkDto> SocialLinks { get; init; } = [];
}

/// <summary>
/// Lightweight DTO for post list views (no full HTML content).
/// </summary>
public sealed record PostSummaryDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Excerpt { get; init; }
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public DateTime? PublishedAt { get; init; }
    public int ViewCount { get; init; }
}

/// <summary>
/// DTO for creating or updating a post.
/// </summary>
public sealed record PostInputDto
{
    public string Title { get; init; } = string.Empty;
    public string ContentHtml { get; init; } = string.Empty;
    public string? Excerpt { get; init; }
    public bool IsPublished { get; init; }
    public List<SocialLinkInputDto> SocialLinks { get; init; } = [];
}
