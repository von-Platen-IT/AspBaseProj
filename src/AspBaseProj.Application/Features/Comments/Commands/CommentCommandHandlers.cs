using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Interfaces;
using AspBaseProj.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspBaseProj.Application.Features.Comments.Commands;

/// <summary>
/// Handler for <see cref="AddCommentCommand"/>.
/// Authenticated user comments are auto-approved; guest comments require moderation.
/// </summary>
public sealed class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IHtmlSanitizerService _sanitizer;

    public AddCommentCommandHandler(IApplicationDbContext context, IHtmlSanitizerService sanitizer)
    {
        _context = context;
        _sanitizer = sanitizer;
    }

    public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var sanitizedContent = _sanitizer.Sanitize(request.Content);
        var isGuest = request.UserId is null;

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = request.PostId,
            ParentCommentId = request.ParentCommentId,
            UserId = request.UserId,
            GuestName = isGuest ? request.GuestName : null,
            GuestEmail = isGuest ? request.GuestEmail : null,
            Content = sanitizedContent,
            IsApproved = !isGuest, // Authenticated users auto-approved
            IsRejected = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);

        string? userName = null;
        if (request.UserId is not null)
        {
            userName = await _context.Users.AsNoTracking()
                .Where(u => u.Id == request.UserId)
                .Select(u => u.DisplayName ?? u.UserName!)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new CommentDto
        {
            Id = comment.Id,
            PostId = comment.PostId,
            ParentCommentId = comment.ParentCommentId,
            UserId = comment.UserId,
            UserName = userName,
            GuestName = comment.GuestName,
            Content = comment.Content,
            IsApproved = comment.IsApproved,
            IsRejected = comment.IsRejected,
            CreatedAt = comment.CreatedAt,
            ApprovedAt = comment.ApprovedAt,
            Replies = []
        };
    }
}

/// <summary>
/// Handler for <see cref="ApproveCommentCommand"/>.
/// </summary>
public sealed class ApproveCommentCommandHandler : IRequestHandler<ApproveCommentCommand>
{
    private readonly IApplicationDbContext _context;

    public ApproveCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ApproveCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Comment with ID {request.CommentId} not found.");

        comment.IsApproved = true;
        comment.IsRejected = false;
        comment.ApprovedAt = DateTime.UtcNow;
        comment.ApprovedById = request.ApprovedById;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="RejectCommentCommand"/>.
/// </summary>
public sealed class RejectCommentCommandHandler : IRequestHandler<RejectCommentCommand>
{
    private readonly IApplicationDbContext _context;

    public RejectCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RejectCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Comment with ID {request.CommentId} not found.");

        comment.IsApproved = false;
        comment.IsRejected = true;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="BulkApproveCommentsCommand"/>.
/// </summary>
public sealed class BulkApproveCommentsCommandHandler : IRequestHandler<BulkApproveCommentsCommand>
{
    private readonly IApplicationDbContext _context;

    public BulkApproveCommentsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(BulkApproveCommentsCommand request, CancellationToken cancellationToken)
    {
        var comments = await _context.Comments
            .Where(c => request.CommentIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var comment in comments)
        {
            comment.IsApproved = true;
            comment.IsRejected = false;
            comment.ApprovedAt = now;
            comment.ApprovedById = request.ApprovedById;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="BulkRejectCommentsCommand"/>.
/// </summary>
public sealed class BulkRejectCommentsCommandHandler : IRequestHandler<BulkRejectCommentsCommand>
{
    private readonly IApplicationDbContext _context;

    public BulkRejectCommentsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(BulkRejectCommentsCommand request, CancellationToken cancellationToken)
    {
        var comments = await _context.Comments
            .Where(c => request.CommentIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        foreach (var comment in comments)
        {
            comment.IsApproved = false;
            comment.IsRejected = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="DeleteCommentCommand"/>.
/// </summary>
public sealed class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Comment with ID {request.CommentId} not found.");

        // Only the comment author, admin, or root can delete
        if (!request.IsCurrentUserRoot && comment.UserId != request.CurrentUserId)
            throw new UnauthorizedAccessException("You can only delete your own comments.");

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
