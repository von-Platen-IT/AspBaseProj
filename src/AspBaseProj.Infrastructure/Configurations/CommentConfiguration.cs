using AspBaseProj.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspBaseProj.Infrastructure.Configurations;

/// <summary>
/// EF Core Fluent API configuration for <see cref="Comment"/>.
/// </summary>
public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.PostId).IsRequired();
        builder.Property(c => c.Content).IsRequired();
        builder.Property(c => c.GuestName).HasMaxLength(100);
        builder.Property(c => c.GuestEmail).HasMaxLength(200);
        builder.Property(c => c.IsApproved).IsRequired();
        builder.Property(c => c.IsRejected).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();

        // Composite index for retrieving comment threads per post
        builder.HasIndex(c => new { c.PostId, c.ParentCommentId });
        // Index for moderation queue queries
        builder.HasIndex(c => c.IsApproved);

        // Self-referencing parent/child for nesting
        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Approved by relationship
        builder.HasOne(c => c.ApprovedBy)
            .WithMany()
            .HasForeignKey(c => c.ApprovedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
