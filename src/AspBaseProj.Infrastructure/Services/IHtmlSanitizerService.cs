using AspBaseProj.Application.Interfaces;

namespace AspBaseProj.Infrastructure.Services;

/// <summary>
/// Service for sanitizing user-generated HTML content to prevent XSS attacks.
/// </summary>
public interface IHtmlSanitizerService : AspBaseProj.Application.Interfaces.IHtmlSanitizerService
{
}
