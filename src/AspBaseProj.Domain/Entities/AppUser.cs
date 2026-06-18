using AspBaseProj.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace AspBaseProj.Domain.Entities;

/// <summary>
/// Represents a registered user of the blog platform.
/// Users are assigned to a group (Author, Admin, Viewer) and the root user
/// has unrestricted superuser privileges (IsRoot = true).
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    /// <summary>
    /// User group for role-based access control.
    /// </summary>
    public UserGroup Group { get; set; } = UserGroup.Viewer;

    /// <summary>
    /// True only for the root superuser account. Bypasses all authorization checks.
    /// </summary>
    public bool IsRoot { get; set; }

    /// <summary>
    /// Optional display name shown publicly.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// FK to Media for user avatar.
    /// </summary>
    public Guid? AvatarImageId { get; set; }

    /// <summary>
    /// Account creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last profile update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Whether the account is active and can log in.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // --- Navigation properties ---

    /// <summary>
    /// Posts authored by this user.
    /// </summary>
    public ICollection<Post> Posts { get; set; } = [];

    /// <summary>
    /// Comments written by this user.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = [];

    /// <summary>
    /// Media files uploaded by this user.
    /// </summary>
    public ICollection<Media> UploadedMedia { get; set; } = [];

    /// <summary>
    /// Avatar media (one-to-one).
    /// </summary>
    public Media? AvatarImage { get; set; }

    /// <summary>
    /// Votes cast by this user.
    /// </summary>
    public ICollection<UserVote> Votes { get; set; } = [];
}
