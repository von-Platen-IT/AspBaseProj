namespace AspBaseProj.Domain.Entities;

/// <summary>
/// Stores uploaded images (post embeds, avatars) directly in the database as binary data.
/// Supports content-type tracking and is linked to the uploading user and optionally a post.
/// </summary>
public class Media
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Original file name.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME type (e.g., image/png, image/jpeg).
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Binary content of the file (stored as bytea in PostgreSQL).
    /// </summary>
    public byte[] Data { get; set; } = [];

    /// <summary>
    /// Size in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// FK to AppUser who uploaded the file.
    /// </summary>
    public Guid UploadedById { get; set; }

    /// <summary>
    /// FK to Post if the media is embedded in a specific post; null for avatars or general uploads.
    /// </summary>
    public Guid? PostId { get; set; }

    /// <summary>
    /// Upload timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // --- Navigation properties ---

    /// <summary>
    /// The user who uploaded this media file.
    /// </summary>
    public AppUser UploadedBy { get; set; } = null!;

    /// <summary>
    /// The post this media belongs to (optional).
    /// </summary>
    public Post? Post { get; set; }
}
