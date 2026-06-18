using AspBaseProj.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspBaseProj.Infrastructure.Configurations;

/// <summary>
/// EF Core Fluent API configuration for <see cref="AppUser"/>.
/// Derived from the database schema single source of truth.
/// </summary>
public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUsers");

        // Unique index on username (Identity uses UserName)
        builder.HasIndex(u => u.UserName).IsUnique();
        // Unique index on email
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Group).HasConversion<int>().IsRequired();
        builder.Property(u => u.IsRoot).IsRequired();
        builder.Property(u => u.DisplayName).HasMaxLength(100);
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.IsActive).IsRequired();

        // Avatar relationship (one-to-one)
        builder.HasOne(u => u.AvatarImage)
            .WithOne()
            .HasForeignKey<AppUser>(u => u.AvatarImageId)
            .OnDelete(DeleteBehavior.SetNull);

        // Posts (one-to-many)
        builder.HasMany(u => u.Posts)
            .WithOne(p => p.Author)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Comments (one-to-many)
        builder.HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Uploaded media (one-to-many)
        builder.HasMany(u => u.UploadedMedia)
            .WithOne(m => m.UploadedBy)
            .HasForeignKey(m => m.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
