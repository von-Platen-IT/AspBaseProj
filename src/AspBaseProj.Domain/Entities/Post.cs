namespace AspBaseProj.Domain.Entities;

/// <summary>
/// A blog post authored by a registered user.
/// Contains HTML content, supports embedded images and social media/video links.
/// </summary>
public class Post
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Post title, max 200 characters.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly unique identifier for the post.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Full HTML body content from rich-text editor.
    /// </summary>
    public string ContentHtml { get; set; } = string.Empty;

    /// <summary>
    /// Optional short summary for list views.
    /// </summary>
    public string? Excerpt { get; set; }

    /// <summary>
    /// FK to AppUser who authored the post.
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Whether the post is publicly visible.
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// When the post was first published.
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last edit timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Number of times the post has been viewed.
    /// </summary>
    public int ViewCount { get; set; }

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
    /// The author of this post.
    /// </summary>
    public AppUser Author { get; set; } = null!;

    /// <summary>
    /// Comments on this post.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = [];

    /// <summary>
    /// Media files embedded in this post.
    /// </summary>
    public ICollection<Media> Media { get; set; } = [];

    /// <summary>
    /// Social/video links embedded in this post.
    /// </summary>
    public ICollection<SocialLink> SocialLinks { get; set; } = [];

    /// <summary>
    /// Votes cast on this post.
    /// </summary>
    public ICollection<UserVote> Votes { get; set; } = [];
}
