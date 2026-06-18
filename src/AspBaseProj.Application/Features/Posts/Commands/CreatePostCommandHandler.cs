using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Interfaces;
using AspBaseProj.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspBaseProj.Application.Features.Posts.Commands;

/// <summary>
/// Handler for <see cref="CreatePostCommand"/>.
/// </summary>
public sealed class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ISlugService _slugService;
    private readonly IHtmlSanitizerService _sanitizer;

    public CreatePostCommandHandler(
        IApplicationDbContext context,
        ISlugService slugService,
        IHtmlSanitizerService sanitizer)
    {
        _context = context;
        _slugService = slugService;
        _sanitizer = sanitizer;
    }

    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var sanitizedHtml = _sanitizer.Sanitize(request.ContentHtml);
        var slug = await _slugService.GenerateUniqueSlugAsync(
            request.Title,
            slug => _context.Posts.AnyAsync(p => p.Slug == slug, cancellationToken),
            cancellationToken);

        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Slug = slug,
            ContentHtml = sanitizedHtml,
            Excerpt = request.Excerpt,
            AuthorId = request.AuthorId,
            IsPublished = request.IsPublished,
            PublishedAt = request.IsPublished ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow,
            ViewCount = 0,
            SocialLinks = request.SocialLinks.Select(s => new SocialLink
            {
                Id = Guid.NewGuid(),
                Url = s.Url,
                Platform = s.Platform,
                Title = s.Title,
                DisplayOrder = s.DisplayOrder,
                CreatedAt = DateTime.UtcNow
            }).ToList()
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync(cancellationToken);

        var author = await _context.Users.AsNoTracking()
            .Where(u => u.Id == request.AuthorId)
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
            AuthorName = author,
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
