using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspBaseProj.Presentation.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AspBaseProj.Presentation.Services;

/// <summary>
/// Implementation of <see cref="IJwtTokenService"/> using symmetric key signing.
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(Guid userId, string userName, string email, string group, bool isRoot)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Email, email),
            new("group", group),
            new("isRoot", isRoot.ToString().ToLowerInvariant())
        };

        if (isRoot)
            claims.Add(new Claim(ClaimTypes.Role, "Root"));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
