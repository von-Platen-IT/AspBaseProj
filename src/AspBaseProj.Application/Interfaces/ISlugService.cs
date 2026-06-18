namespace AspBaseProj.Application.Interfaces;

/// <summary>
/// Service for generating URL-friendly slugs from post titles.
/// Defined in Application layer for dependency inversion.
/// </summary>
public interface ISlugService
{
    /// <summary>
    /// Generates a URL-friendly slug from the given title.
    /// </summary>
    string GenerateSlug(string title);

    /// <summary>
    /// Generates a unique slug by appending a suffix if the slug already exists in the database.
    /// </summary>
    Task<string> GenerateUniqueSlugAsync(string title, Func<string, Task<bool>> slugExists, CancellationToken cancellationToken = default);
}
