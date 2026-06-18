namespace AspBaseProj.Infrastructure.Services;

/// <summary>
/// Service for generating URL-friendly slugs from post titles.
/// Inherits from the Application layer interface for dependency inversion.
/// </summary>
public interface ISlugService : AspBaseProj.Application.Interfaces.ISlugService
{
}
