using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Features.Posts.Queries;
using AspBaseProj.Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspBaseProj.Presentation.Pages.Admin;

[Authorize(Policy = Policies.Admin)]
public class ContentModel : PageModel
{
    private readonly IMediator _mediator;

    public ContentModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public List<PostSummaryDto> Posts { get; set; } = [];

    public async Task OnGetAsync()
    {
        Posts = await _mediator.Send(new GetAllPostsQuery { IncludeUnpublished = true });
    }
}
