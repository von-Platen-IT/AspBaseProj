namespace AspBaseProj.Domain.Entities;

/// <summary>
/// A comment on a blog post or a reply to another comment (nested discussions).
/// Authenticated users' comments are immediately visible; guest comments require
/// moderation approval.
/// </summary>
public class Comment
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// FK to the Post being commented on.
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// FK to parent Comment for nested replies; null for top-level comments.
    /// </summary>
    public Guid? ParentCommentId { get; set; }

    /// <summary>
    /// FK to AppUser if commenter is authenticated; null for guests.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Name provided by guest commenter; null for authenticated users.
    /// </summary>
    public string? GuestName { get; set; }

    /// <summary>
    /// Email provided by guest commenter; null for authenticated users.
    /// </summary>
    public string? GuestEmail { get; set; }

    /// <summary>
    /// Comment text content.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// True if visible publicly. Authenticated user comments are auto-approved;
    /// guest comments start false.
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// True if a moderator rejected this comment.
    /// </summary>
    public bool IsRejected { get; set; }

    /// <summary>
    /// Comment creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the comment was approved by a moderator.
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// FK to AppUser (Admin/root) who approved the comment.
    /// </summary>
    public Guid? ApprovedById { get; set; }

    /// <summary>
    /// Total number of upvotes.
    /// </summary>
    public int UpvoteCount { get; set; }

    /// <summary>
    /// Total number of downvotes.
    /// </summary>
    public int DownvoteCount { get; set; }

    // --- Navigation properties ---

    /// <summary>
    /// The post this comment belongs to.
    /// </summary>
    public Post Post { get; set; } = null!;

    /// The admin/root who approved the comment (optional).
    /// Parent comment for nesting (null for top-level).
    /// </summary>
    public Comment? ParentComment { get; set; }

    /// <summary>
    /// Child replies (nested comments).
    /// </summary>
    public ICollection<Comment> Replies { get; set; } = [];

    /// <summary>
    /// The authenticated user who wrote the comment (optional).
    /// </summary>
    public AppUser? User { get; set; }

    /// <summary>
    /// The admin/root who approved the comment (optional).
    /// </summary>
    public AppUser? ApprovedBy { get; set; }

    /// <summary>
    /// Votes cast on this comment.
    /// </summary>
    public ICollection<UserVote> Votes { get; set; } = [];
}
