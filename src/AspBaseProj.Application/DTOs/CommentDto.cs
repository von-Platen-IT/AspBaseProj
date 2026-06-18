namespace AspBaseProj.Application.DTOs;

/// <summary>
/// DTO for a comment with nested replies (threaded display).
/// </summary>
public sealed record CommentDto
{
    public Guid Id { get; init; }
    public Guid PostId { get; init; }
    public Guid? ParentCommentId { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public string? GuestName { get; init; }
    public string Content { get; init; } = string.Empty;
    public bool IsApproved { get; init; }
    public bool IsRejected { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public int UpvoteCount { get; init; }
    public int DownvoteCount { get; init; }
    public List<CommentDto> Replies { get; init; } = [];
}

/// <summary>
/// DTO for creating a comment.
/// </summary>
public sealed record CommentInputDto
{
    public Guid PostId { get; init; }
    public Guid? ParentCommentId { get; init; }
    public string Content { get; init; } = string.Empty;
    // Guest fields (only used when user is not authenticated)
    public string? GuestName { get; init; }
    public string? GuestEmail { get; init; }
}

/// <summary>
/// DTO for a pending comment in the moderation queue.
/// </summary>
public sealed record PendingCommentDto
{
    public Guid Id { get; init; }
    public Guid PostId { get; init; }
    public string PostTitle { get; init; } = string.Empty;
    public string? GuestName { get; init; }
    public string? GuestEmail { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
