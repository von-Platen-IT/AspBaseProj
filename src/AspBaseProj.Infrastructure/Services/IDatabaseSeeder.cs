namespace AspBaseProj.Infrastructure.Services;

/// <summary>
/// Service for seeding the database with initial data (root user, default system settings).
/// </summary>
public interface IDatabaseSeeder
{
    /// <summary>
    /// Seeds the database with the root user and default system settings if they don't exist.
    /// </summary>
    Task SeedAsync(string rootUserName, string rootEmail, string rootPassword, CancellationToken cancellationToken = default);
}
