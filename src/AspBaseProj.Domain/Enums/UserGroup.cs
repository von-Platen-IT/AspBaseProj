namespace AspBaseProj.Domain.Enums;

/// <summary>
/// User group for role-based access control.
/// The root user is identified by <see cref="Entities.AppUser.IsRoot"/> = true, not by this enum.
/// </summary>
public enum UserGroup
{
    /// <summary>
    /// Can create, edit, and delete their own blog posts.
    /// </summary>
    Author = 1,

    /// <summary>
    /// Can moderate comments, manage content, and access the admin dashboard.
    /// </summary>
    Admin = 2,

    /// <summary>
    /// Read-only access; default group for new registrations.
    /// </summary>
    Viewer = 3
}
