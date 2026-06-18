using AspBaseProj.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspBaseProj.Infrastructure.Configurations;

/// <summary>
/// EF Core Fluent API configuration for <see cref="Post"/>.
/// </summary>
public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Slug).HasMaxLength(250).IsRequired();
        builder.Property(p => p.ContentHtml).IsRequired();
        builder.Property(p => p.Excerpt).HasMaxLength(500);
        builder.Property(p => p.AuthorId).IsRequired();
        builder.Property(p => p.IsPublished).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.ViewCount).IsRequired();

        // Unique index on slug for URL routing
        builder.HasIndex(p => p.Slug).IsUnique();
        // Index for querying posts by author
        builder.HasIndex(p => p.AuthorId);
        // Index for chronological listing
        builder.HasIndex(p => p.PublishedAt);

        // Comments (one-to-many)
        builder.HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Media (one-to-many)
        builder.HasMany(p => p.Media)
            .WithOne(m => m.Post)
            .HasForeignKey(m => m.PostId)
            .OnDelete(DeleteBehavior.SetNull);

        // Social links (one-to-many)
        builder.HasMany(p => p.SocialLinks)
            .WithOne(s => s.Post)
            .HasForeignKey(s => s.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
