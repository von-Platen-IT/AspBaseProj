using AspBaseProj.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspBaseProj.Application.Interfaces;

/// <summary>
/// Abstraction for the application's database context, defined in the Application layer
/// to maintain dependency inversion (Application does not depend on Infrastructure).
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Post> Posts { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Media> Media { get; }
    DbSet<SystemSetting> SystemSettings { get; }
    DbSet<SocialLink> SocialLinks { get; }
    DbSet<UserVote> UserVotes { get; }
    DbSet<AppUser> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
