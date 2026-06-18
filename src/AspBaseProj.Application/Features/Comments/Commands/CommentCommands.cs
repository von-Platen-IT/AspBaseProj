using AspBaseProj.Application.DTOs;
using MediatR;

namespace AspBaseProj.Application.Features.Comments.Commands;

/// <summary>
/// Command to add a comment to a post (authenticated or guest).
/// </summary>
public sealed record AddCommentCommand : IRequest<CommentDto>
{
    public Guid PostId { get; init; }
    public Guid? ParentCommentId { get; init; }
    public Guid? UserId { get; init; }
    public string? GuestName { get; init; }
    public string? GuestEmail { get; init; }
    public string Content { get; init; } = string.Empty;
}

/// <summary>
/// Command to approve a comment (admin/root).
/// </summary>
public sealed record ApproveCommentCommand : IRequest
{
    public Guid CommentId { get; init; }
    public Guid ApprovedById { get; init; }
}

/// <summary>
/// Command to reject a comment (admin/root).
/// </summary>
public sealed record RejectCommentCommand : IRequest
{
    public Guid CommentId { get; init; }
}

/// <summary>
/// Command to bulk approve comments.
/// </summary>
public sealed record BulkApproveCommentsCommand : IRequest
{
    public List<Guid> CommentIds { get; init; } = [];
    public Guid ApprovedById { get; init; }
}

/// <summary>
/// Command to bulk reject comments.
/// </summary>
public sealed record BulkRejectCommentsCommand : IRequest
{
    public List<Guid> CommentIds { get; init; } = [];
}

/// <summary>
/// Command to delete a comment.
/// </summary>
public sealed record DeleteCommentCommand : IRequest
{
    public Guid CommentId { get; init; }
    public Guid CurrentUserId { get; init; }
    public bool IsCurrentUserRoot { get; init; }
}
