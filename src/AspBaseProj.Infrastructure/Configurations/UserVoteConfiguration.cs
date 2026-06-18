using AspBaseProj.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspBaseProj.Infrastructure.Configurations;

/// <summary>
/// EF Core Fluent API configuration for <see cref="UserVote"/>.
/// </summary>
public sealed class UserVoteConfiguration : IEntityTypeConfiguration<UserVote>
{
    public void Configure(EntityTypeBuilder<UserVote> builder)
    {
        builder.ToTable("UserVotes");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.UserId).IsRequired();
        builder.Property(v => v.IsUpvote).IsRequired();
        builder.Property(v => v.CreatedAt).IsRequired();

        // A vote must reference either a Post OR a Comment (not both, not neither)
        // This is enforced by application logic; EF checks uniqueness per index below.

        // One user can only vote once per post
        builder.HasIndex(v => new { v.UserId, v.PostId })
            .IsUnique()
            .HasFilter("\"PostId\" IS NOT NULL");

        // One user can only vote once per comment
        builder.HasIndex(v => new { v.UserId, v.CommentId })
            .IsUnique()
            .HasFilter("\"CommentId\" IS NOT NULL");

        // Relationships
        builder.HasOne(v => v.User)
            .WithMany(u => u.Votes)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Post)
            .WithMany(p => p.Votes)
            .HasForeignKey(v => v.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Comment)
            .WithMany(c => c.Votes)
            .HasForeignKey(v => v.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}