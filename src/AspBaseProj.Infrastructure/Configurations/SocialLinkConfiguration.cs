using AspBaseProj.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspBaseProj.Infrastructure.Configurations;

/// <summary>
/// EF Core Fluent API configuration for <see cref="SocialLink"/>.
/// </summary>
public sealed class SocialLinkConfiguration : IEntityTypeConfiguration<SocialLink>
{
    public void Configure(EntityTypeBuilder<SocialLink> builder)
    {
        builder.ToTable("SocialLinks");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.PostId).IsRequired();
        builder.Property(s => s.Url).HasMaxLength(2000).IsRequired();
        builder.Property(s => s.Platform).HasMaxLength(50);
        builder.Property(s => s.Title).HasMaxLength(200);
        builder.Property(s => s.DisplayOrder).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();

        builder.HasIndex(s => s.PostId);
    }
}
