using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Features.Posts.Queries;
using AspBaseProj.Application.Features.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspBaseProj.Presentation.Pages;

public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public PagedResult<PostSummaryDto> Posts { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string BlogTitle { get; set; } = "AspBaseProj Blog";

    public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
    {
        SearchTerm = search;

        // Load blog title from settings
        var blogTitle = await _mediator.Send(new GetSettingByKeyQuery { Key = "BlogTitle" });
        if (!string.IsNullOrEmpty(blogTitle))
            BlogTitle = blogTitle;

        ViewData["BlogTitle"] = BlogTitle;

        Posts = await _mediator.Send(new GetPostListQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = search
        });
    }
}
