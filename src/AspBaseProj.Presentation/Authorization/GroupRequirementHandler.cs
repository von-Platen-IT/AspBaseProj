using AspBaseProj.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AspBaseProj.Presentation.Authorization;

/// <summary>
/// Authorization handler for <see cref="GroupRequirement"/>.
/// Root users (IsRoot = true) bypass all group checks.
/// </summary>
public sealed class GroupRequirementHandler : AuthorizationHandler<GroupRequirement>
{
    private readonly UserManager<AppUser> _userManager;

    public GroupRequirementHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        GroupRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return;

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null || !user.IsActive)
            return;

        // Root users bypass all authorization checks
        if (user.IsRoot)
        {
            context.Succeed(requirement);
            return;
        }

        // If root is explicitly required, non-root users fail
        if (requirement.RequireRoot)
            return;

        // Check if the user's group is in the allowed groups
        if (requirement.AllowedGroups.Contains(user.Group))
        {
            context.Succeed(requirement);
        }
    }
}
