namespace AspBaseProj.Application.Features.Votes.Commands;

using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Interfaces;
using AspBaseProj.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Handler for <see cref="VoteOnPostCommand"/>.
/// </summary>
public sealed class VoteOnPostCommandHandler : IRequestHandler<VoteOnPostCommand, VoteDto>
{
    private readonly IApplicationDbContext _context;

    public VoteOnPostCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VoteDto> Handle(VoteOnPostCommand request, CancellationToken cancellationToken)
    {
        var existingVote = await _context.UserVotes
            .Where(v => v.UserId == request.UserId && v.PostId == request.PostId)
            .FirstOrDefaultAsync(cancellationToken);

        var post = await _context.Posts
            .FirstAsync(p => p.Id == request.PostId, cancellationToken);

        if (existingVote is not null)
        {
            if (existingVote.IsUpvote == request.IsUpvote)
            {
                // Same vote → remove it (toggle off)
                _context.UserVotes.Remove(existingVote);
                if (request.IsUpvote)
                    post.UpvoteCount = Math.Max(0, post.UpvoteCount - 1);
                else
                    post.DownvoteCount = Math.Max(0, post.DownvoteCount - 1);
            }
            else
            {
                // Different vote → change direction
                if (request.IsUpvote)
                {
                    post.DownvoteCount = Math.Max(0, post.DownvoteCount - 1);
                    post.UpvoteCount++;
                }
                else
                {
                    post.UpvoteCount = Math.Max(0, post.UpvoteCount - 1);
                    post.DownvoteCount++;
                }
                existingVote.IsUpvote = request.IsUpvote;
                existingVote.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            // New vote
            var vote = new UserVote
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PostId = request.PostId,
                IsUpvote = request.IsUpvote,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserVotes.Add(vote);

            if (request.IsUpvote)
                post.UpvoteCount++;
            else
                post.DownvoteCount++;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new VoteDto
        {
            HasVoted = existingVote is null || existingVote.IsUpvote != request.IsUpvote,
            IsUpvote = request.IsUpvote,
            UpvoteCount = post.UpvoteCount,
            DownvoteCount = post.DownvoteCount
        };
    }
}

/// <summary>
/// Handler for <see cref="VoteOnCommentCommand"/>.
/// </summary>
public sealed class VoteOnCommentCommandHandler : IRequestHandler<VoteOnCommentCommand, VoteDto>
{
    private readonly IApplicationDbContext _context;

    public VoteOnCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VoteDto> Handle(VoteOnCommentCommand request, CancellationToken cancellationToken)
    {
        var existingVote = await _context.UserVotes
            .Where(v => v.UserId == request.UserId && v.CommentId == request.CommentId)
            .FirstOrDefaultAsync(cancellationToken);

        var comment = await _context.Comments
            .FirstAsync(c => c.Id == request.CommentId, cancellationToken);

        if (existingVote is not null)
        {
            if (existingVote.IsUpvote == request.IsUpvote)
            {
                // Toggle off
                _context.UserVotes.Remove(existingVote);
                if (request.IsUpvote)
                    comment.UpvoteCount = Math.Max(0, comment.UpvoteCount - 1);
                else
                    comment.DownvoteCount = Math.Max(0, comment.DownvoteCount - 1);
            }
            else
            {
                // Change direction
                if (request.IsUpvote)
                {
                    comment.DownvoteCount = Math.Max(0, comment.DownvoteCount - 1);
                    comment.UpvoteCount++;
                }
                else
                {
                    comment.UpvoteCount = Math.Max(0, comment.UpvoteCount - 1);
                    comment.DownvoteCount++;
                }
                existingVote.IsUpvote = request.IsUpvote;
                existingVote.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            var vote = new UserVote
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                CommentId = request.CommentId,
                IsUpvote = request.IsUpvote,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserVotes.Add(vote);

            if (request.IsUpvote)
                comment.UpvoteCount++;
            else
                comment.DownvoteCount++;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new VoteDto
        {
            HasVoted = existingVote is null || existingVote.IsUpvote != request.IsUpvote,
            IsUpvote = request.IsUpvote,
            UpvoteCount = existingVote?.IsUpvote == request.IsUpvote
                ? (request.IsUpvote ? comment.UpvoteCount : comment.DownvoteCount)
                : comment.UpvoteCount,
            DownvoteCount = comment.DownvoteCount
        };
    }
}
