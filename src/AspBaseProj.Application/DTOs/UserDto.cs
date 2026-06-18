using AspBaseProj.Domain.Enums;

namespace AspBaseProj.Application.DTOs;

/// <summary>
/// DTO for a user in management/admin views.
/// </summary>
public sealed record UserDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool EmailConfirmed { get; init; }
    public UserGroup Group { get; init; }
    public bool IsRoot { get; init; }
    public string? DisplayName { get; init; }
    public Guid? AvatarImageId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO for updating a user's group (root-only).
/// </summary>
public sealed record UpdateUserGroupDto
{
    public Guid UserId { get; init; }
    public UserGroup Group { get; init; }
}

/// <summary>
/// DTO for activating/deactivating a user.
/// </summary>
public sealed record UpdateUserStatusDto
{
    public Guid UserId { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO for updating user profile.
/// </summary>
public sealed record UpdateProfileDto
{
    public string? DisplayName { get; init; }
    public string Email { get; init; } = string.Empty;
}
