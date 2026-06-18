using System.Security.Claims;

namespace AspBaseProj.Presentation.Services;

/// <summary>
/// Service for generating JWT tokens for API authentication.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    string GenerateToken(Guid userId, string userName, string email, string group, bool isRoot);
}
