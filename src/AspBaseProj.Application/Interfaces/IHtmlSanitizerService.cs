namespace AspBaseProj.Application.Interfaces;

/// <summary>
/// Service for sanitizing user-generated HTML content to prevent XSS attacks.
/// Defined in Application layer for dependency inversion.
/// </summary>
public interface IHtmlSanitizerService
{
    /// <summary>
    /// Sanitizes the given HTML string, removing dangerous tags and attributes.
    /// </summary>
    string Sanitize(string html);
}
