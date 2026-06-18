using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspBaseProj.Application.Features.Comments.Queries;

/// <summary>
/// Query to get threaded comments for a post (approved only for public view).
/// </summary>
public sealed record GetCommentsByPostQuery : IRequest<List<CommentDto>>
{
    public Guid PostId { get; init; }
    public bool IncludeUnapproved { get; init; }
}

/// <summary>
/// Query to get pending guest comments for the moderation queue.
/// </summary>
public sealed record GetPendingCommentsQuery : IRequest<List<PendingCommentDto>>;

/// <summary>
/// Handler for <see cref="GetCommentsByPostQuery"/>.
/// Builds a nested comment tree from flat data.
/// </summary>
public sealed class GetCommentsByPostQueryHandler : IRequestHandler<GetCommentsByPostQuery, List<CommentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCommentsByPostQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CommentDto>> Handle(GetCommentsByPostQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.PostId == request.PostId);

        if (!request.IncludeUnapproved)
            query = query.Where(c => c.IsApproved && !c.IsRejected);

        var comments = await query
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                PostId = c.PostId,
                ParentCommentId = c.ParentCommentId,
                UserId = c.UserId,
                UserName = c.User != null ? (c.User.DisplayName ?? c.User.UserName!) : null,
                GuestName = c.GuestName,
                Content = c.Content,
                IsApproved = c.IsApproved,
                IsRejected = c.IsRejected,
                CreatedAt = c.CreatedAt,
                ApprovedAt = c.ApprovedAt,
                Replies = new List<CommentDto>()
            })
            .ToListAsync(cancellationToken);

        // Build nested tree
        var lookup = comments.ToLookup(c => c.ParentCommentId);
        foreach (var comment in comments)
        {
            var replies = lookup[comment.Id].ToList();
            // Use reflection-free approach: rebuild with replies
            // Since records are immutable, we need to rebuild
        }

        // Return only top-level comments (ParentCommentId == null)
        // The nested replies are populated via the lookup
        return BuildTree(comments, null);
    }

    private static List<CommentDto> BuildTree(List<CommentDto> allComments, Guid? parentId)
    {
        var children = allComments.Where(c => c.ParentCommentId == parentId).ToList();
        foreach (var child in children)
        {
            var replies = BuildTree(allComments, child.Id);
            // Rebuild the comment with its replies
            var index = children.IndexOf(child);
            children[index] = child with { Replies = replies };
        }
        return children;
    }
}

/// <summary>
/// Handler for <see cref="GetPendingCommentsQuery"/>.
/// </summary>
public sealed class GetPendingCommentsQueryHandler : IRequestHandler<GetPendingCommentsQuery, List<PendingCommentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingCommentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PendingCommentDto>> Handle(GetPendingCommentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Comments
            .AsNoTracking()
            .Where(c => !c.IsApproved && !c.IsRejected && c.UserId == null)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new PendingCommentDto
            {
                Id = c.Id,
                PostId = c.PostId,
                PostTitle = c.Post.Title,
                GuestName = c.GuestName,
                GuestEmail = c.GuestEmail,
                Content = c.Content,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
