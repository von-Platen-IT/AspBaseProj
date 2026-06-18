using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Interfaces;
using AspBaseProj.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspBaseProj.Application.Features.Posts.Commands;

/// <summary>
/// Handler for <see cref="UpdatePostCommand"/>.
/// Authors can only edit their own posts; root can edit any post.
/// </summary>
public sealed class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, PostDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ISlugService _slugService;
    private readonly IHtmlSanitizerService _sanitizer;

    public UpdatePostCommandHandler(
        IApplicationDbContext context,
        ISlugService slugService,
        IHtmlSanitizerService sanitizer)
    {
        _context = context;
        _slugService = slugService;
        _sanitizer = sanitizer;
    }

    public async Task<PostDto> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .Include(p => p.SocialLinks)
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken)
            ?? throw new KeyNotFoundException($"Post with ID {request.PostId} not found.");

        // Authorization: author can only edit own posts, root can edit any
        if (!request.IsCurrentUserRoot && post.AuthorId != request.CurrentUserId)
            throw new UnauthorizedAccessException("You can only edit your own posts.");

        var sanitizedHtml = _sanitizer.Sanitize(request.ContentHtml);

        // Update slug only if title changed
        if (!string.Equals(post.Title, request.Title, StringComparison.OrdinalIgnoreCase))
        {
            post.Slug = await _slugService.GenerateUniqueSlugAsync(
                request.Title,
                slug => _context.Posts.AnyAsync(p => p.Slug == slug && p.Id != post.Id, cancellationToken),
                cancellationToken);
        }

        post.Title = request.Title;
        post.ContentHtml = sanitizedHtml;
        post.Excerpt = request.Excerpt;
        post.UpdatedAt = DateTime.UtcNow;

        // Handle publish state transition
        if (request.IsPublished && !post.IsPublished)
        {
            post.IsPublished = true;
            post.PublishedAt ??= DateTime.UtcNow;
        }
        else if (!request.IsPublished)
        {
            post.IsPublished = false;
        }

        // Update social links (replace all)
        post.SocialLinks.Clear();
        foreach (var s in request.SocialLinks)
        {
            post.SocialLinks.Add(new SocialLink
            {
                Id = Guid.NewGuid(),
                Url = s.Url,
                Platform = s.Platform,
                Title = s.Title,
                DisplayOrder = s.DisplayOrder,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var authorName = await _context.Users.AsNoTracking()
            .Where(u => u.Id == post.AuthorId)
            .Select(u => u.DisplayName ?? u.UserName!)
            .FirstAsync(cancellationToken);

        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Slug = post.Slug,
            ContentHtml = post.ContentHtml,
            Excerpt = post.Excerpt,
            AuthorId = post.AuthorId,
            AuthorName = authorName,
            IsPublished = post.IsPublished,
            PublishedAt = post.PublishedAt,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            ViewCount = post.ViewCount,
            SocialLinks = post.SocialLinks.Select(s => new SocialLinkDto
            {
                Id = s.Id,
                PostId = post.Id,
                Url = s.Url,
                Platform = s.Platform,
                Title = s.Title,
                DisplayOrder = s.DisplayOrder
            }).ToList()
        };
    }
}

/// <summary>
/// Handler for <see cref="DeletePostCommand"/>.
/// </summary>
public sealed class DeletePostCommandHandler : IRequestHandler<DeletePostCommand>
{
    private readonly IApplicationDbContext _context;

    public DeletePostCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken)
            ?? throw new KeyNotFoundException($"Post with ID {request.PostId} not found.");

        if (!request.IsCurrentUserRoot && post.AuthorId != request.CurrentUserId)
            throw new UnauthorizedAccessException("You can only delete your own posts.");

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="IncrementViewCountCommand"/>.
/// </summary>
public sealed class IncrementViewCountCommandHandler : IRequestHandler<IncrementViewCountCommand>
{
    private readonly IApplicationDbContext _context;

    public IncrementViewCountCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(IncrementViewCountCommand request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

        if (post is not null)
        {
            post.ViewCount++;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
