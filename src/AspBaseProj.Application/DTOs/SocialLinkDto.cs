namespace AspBaseProj.Application.DTOs;

/// <summary>
/// DTO for a social media/video link embedded in a post.
/// </summary>
public sealed record SocialLinkDto
{
    public Guid Id { get; init; }
    public Guid PostId { get; init; }
    public string Url { get; init; } = string.Empty;
    public string? Platform { get; init; }
    public string? Title { get; init; }
    public int DisplayOrder { get; init; }
}

/// <summary>
/// DTO for creating/updating a social link.
/// </summary>
public sealed record SocialLinkInputDto
{
    public string Url { get; init; } = string.Empty;
    public string? Platform { get; init; }
    public string? Title { get; init; }
    public int DisplayOrder { get; init; }
}
