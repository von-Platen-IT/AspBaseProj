namespace AspBaseProj.Domain.Entities;

/// <summary>
/// Represents a social media or video platform link embedded in a post,
/// rendered as a visually appealing link card.
/// </summary>
public class SocialLink
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// FK to the Post containing this link.
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// The full URL to the social media or video resource.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Detected or specified platform name (e.g., YouTube, Twitter, Vimeo).
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// Optional display title for the link card.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Ordering of links within the post.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // --- Navigation properties ---

    /// <summary>
    /// The post this social link belongs to.
    /// </summary>
    public Post Post { get; set; } = null!;
}
