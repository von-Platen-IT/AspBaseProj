using MediatR;

namespace AspBaseProj.Application.DTOs;

/// <summary>
/// DTO for a user's vote on a post or comment.
/// </summary>
public sealed record VoteDto
{
    /// <summary>
    /// Whether the current user has voted on this item.
    /// </summary>
    public bool HasVoted { get; init; }

    /// <summary>
    /// True if the current user's vote is an upvote.
    /// </summary>
    public bool IsUpvote { get; init; }

    /// <summary>
    /// Total number of upvotes.
    /// </summary>
    public int UpvoteCount { get; init; }

    /// <summary>
    /// Total number of downvotes.
    /// </summary>
    public int DownvoteCount { get; init; }
}

/// <summary>
/// Request to vote on a post.
/// </summary>
public sealed record VoteOnPostCommand : IRequest<VoteDto>
{
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }
    public bool IsUpvote { get; init; }
}

/// <summary>
/// Request to vote on a comment.
/// </summary>
public sealed record VoteOnCommentCommand : IRequest<VoteDto>
{
    public Guid CommentId { get; init; }
    public Guid UserId { get; init; }
    public bool IsUpvote { get; init; }
}

/// <summary>
/// Request to get the current user's votes for a post and its comments.
/// </summary>
public sealed record GetVotesForPostQuery : IRequest<Dictionary<string, object>>
{
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }
}

/// <summary>
/// DTO representing a user vote on a comment for the client.
/// </summary>
public sealed record CommentVoteDto
{
    public Guid CommentId { get; init; }
    public bool IsUpvote { get; init; }
}
