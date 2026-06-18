namespace AspBaseProj.Domain.Entities;

/// <summary>
/// Represents a vote (upvote or downvote) cast by a user on a post or comment.
/// Each user may have at most one vote per post and per comment.
/// </summary>
public class UserVote
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// FK to the user who cast the vote.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// FK to the Post being voted on (null if voting on a comment).
    /// </summary>
    public Guid? PostId { get; set; }

    /// <summary>
    /// FK to the Comment being voted on (null if voting on a post).
    /// </summary>
    public Guid? CommentId { get; set; }

    /// <summary>
    /// True for upvote, false for downvote.
    /// </summary>
    public bool IsUpvote { get; set; }

    /// <summary>
    /// When the vote was cast.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the vote was last changed.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // --- Navigation properties ---

    /// <summary>
    /// The user who cast this vote.
    /// </summary>
    public AppUser User { get; set; } = null!;

    /// <summary>
    /// The post being voted on (null if comment vote).
    /// </summary>
    public Post? Post { get; set; }

    /// <summary>
    /// The comment being voted on (null if post vote).
    /// </summary>
    public Comment? Comment { get; set; }
}