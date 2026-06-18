using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspBaseProj.Application.Features.Posts.Queries;

/// <summary>
/// Handler for <see cref="GetPostListQuery"/>.
/// Returns a paginated list of published posts with search support.
/// </summary>
public sealed class GetPostListQueryHandler : IRequestHandler<GetPostListQuery, PagedResult<PostSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPostListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PostSummaryDto>> Handle(GetPostListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Posts
            .AsNoTracking()
            .Where(p => p.IsPublished);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(p => p.Title.Contains(term) || p.ContentHtml.Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .OrderByDescending(p => p.PublishedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PostSummaryDto
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Excerpt = p.Excerpt,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.DisplayName ?? p.Author.UserName!,
                PublishedAt = p.PublishedAt,
                ViewCount = p.ViewCount
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PostSummaryDto>
        {
            Items = posts,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

/// <summary>
/// Handler for <see cref="GetPostBySlugQuery"/>.
/// </summary>
public sealed class GetPostBySlugQueryHandler : IRequestHandler<GetPostBySlugQuery, PostDto>
{
    private readonly IApplicationDbContext _context;

    public GetPostBySlugQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PostDto> Handle(GetPostBySlugQuery request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .Include(p => p.SocialLinks)
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken)
            ?? throw new KeyNotFoundException($"Post with slug '{request.Slug}' not found.");

        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Slug = post.Slug,
            ContentHtml = post.ContentHtml,
            Excerpt = post.Excerpt,
            AuthorId = post.AuthorId,
            AuthorName = post.Author.DisplayName ?? post.Author.UserName!,
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
/// Handler for <see cref="GetPostsByAuthorQuery"/>.
/// </summary>
public sealed class GetPostsByAuthorQueryHandler : IRequestHandler<GetPostsByAuthorQuery, List<PostSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPostsByAuthorQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PostSummaryDto>> Handle(GetPostsByAuthorQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Posts
            .AsNoTracking()
            .Where(p => p.AuthorId == request.AuthorId);

        if (!request.IncludeUnpublished)
            query = query.Where(p => p.IsPublished);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostSummaryDto
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Excerpt = p.Excerpt,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.DisplayName ?? p.Author.UserName!,
                PublishedAt = p.PublishedAt,
                ViewCount = p.ViewCount
            })
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="GetAllPostsQuery"/>.
/// </summary>
public sealed class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, List<PostSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllPostsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PostSummaryDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Posts.AsNoTracking();

        if (!request.IncludeUnpublished)
            query = query.Where(p => p.IsPublished);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostSummaryDto
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Excerpt = p.Excerpt,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.DisplayName ?? p.Author.UserName!,
                PublishedAt = p.PublishedAt,
                ViewCount = p.ViewCount
            })
            .ToListAsync(cancellationToken);
    }
}
