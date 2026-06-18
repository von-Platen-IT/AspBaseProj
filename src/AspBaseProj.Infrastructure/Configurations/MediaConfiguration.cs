using AspBaseProj.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspBaseProj.Infrastructure.Configurations;

/// <summary>
/// EF Core Fluent API configuration for <see cref="Media"/>.
/// Binary data is stored as bytea in PostgreSQL.
/// </summary>
public sealed class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.ToTable("Media");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.FileName).HasMaxLength(255).IsRequired();
        builder.Property(m => m.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(m => m.Data).IsRequired();
        builder.Property(m => m.FileSize).IsRequired();
        builder.Property(m => m.UploadedById).IsRequired();
        builder.Property(m => m.CreatedAt).IsRequired();

        // Index for retrieving media for a post
        builder.HasIndex(m => m.PostId);
    }
}
