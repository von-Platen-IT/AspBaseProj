using AspBaseProj.Domain.Entities;
using AspBaseProj.Domain.Enums;
using AspBaseProj.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AspBaseProj.Infrastructure.Services;

/// <summary>
/// Implementation of <see cref="IDatabaseSeeder"/> that seeds the root user
/// and default system settings.
/// </summary>
public sealed class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext context,
        UserManager<AppUser> userManager,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync(string rootUserName, string rootEmail, string rootPassword, CancellationToken cancellationToken = default)
    {
        await SeedRootUserAsync(rootUserName, rootEmail, rootPassword, cancellationToken);
        await SeedSystemSettingsAsync(cancellationToken);
    }

    private async Task SeedRootUserAsync(string rootUserName, string rootEmail, string rootPassword, CancellationToken cancellationToken)
    {
        var existing = await _userManager.FindByNameAsync(rootUserName);
        if (existing is not null)
        {
            _logger.LogInformation("Root user '{RootUserName}' already exists, skipping seed", rootUserName);
            return;
        }

        var rootUser = new AppUser
        {
            UserName = rootUserName,
            Email = rootEmail,
            EmailConfirmed = true,
            Group = UserGroup.Admin,
            IsRoot = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            DisplayName = "Root Superuser"
        };

        var result = await _userManager.CreateAsync(rootUser, rootPassword);
        if (result.Succeeded)
        {
            _logger.LogInformation("Root user '{RootUserName}' created successfully", rootUserName);
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError("Failed to create root user '{RootUserName}': {Errors}", rootUserName, errors);
        }
    }

    private async Task SeedSystemSettingsAsync(CancellationToken cancellationToken)
    {
        var defaultSettings = new[]
        {
            new SystemSetting
            {
                Key = "BlogTitle",
                Value = "AspBaseProj Blog",
                Description = "The title of the blog displayed in the header."
            },
            new SystemSetting
            {
                Key = "ModerationEnabled",
                Value = "true",
                Description = "Whether guest comments require moderation approval before being visible."
            }
        };

        foreach (var setting in defaultSettings)
        {
            var exists = await _context.SystemSettings.AnyAsync(s => s.Key == setting.Key, cancellationToken);
            if (!exists)
            {
                _context.SystemSettings.Add(setting);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("System settings seeded successfully");
    }
}
