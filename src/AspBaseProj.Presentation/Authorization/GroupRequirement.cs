using AspBaseProj.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace AspBaseProj.Presentation.Authorization;

/// <summary>
/// Authorization requirement that checks the user's group and optionally requires root.
/// Root users (IsRoot = true) bypass all group checks.
/// </summary>
public sealed class GroupRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The groups that satisfy this requirement.
    /// </summary>
    public UserGroup[] AllowedGroups { get; }

    /// <summary>
    /// If true, only root users (IsRoot = true) satisfy this requirement.
    /// </summary>
    public bool RequireRoot { get; }

    /// <summary>
    /// Creates a requirement for the specified groups. Root users always pass.
    /// </summary>
    public GroupRequirement(params UserGroup[] allowedGroups)
    {
        AllowedGroups = allowedGroups;
        RequireRoot = false;
    }

    /// <summary>
    /// Creates a requirement that only root users can satisfy.
    /// </summary>
    public GroupRequirement(UserGroup group, bool requireRoot)
    {
        AllowedGroups = [group];
        RequireRoot = requireRoot;
    }
}
