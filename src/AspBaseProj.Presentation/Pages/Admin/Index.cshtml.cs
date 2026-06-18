using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Features.Comments.Queries;
using AspBaseProj.Application.Features.Users;
using AspBaseProj.Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspBaseProj.Presentation.Pages.Admin;

[Authorize(Policy = Policies.Admin)]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public DashboardStatsDto Stats { get; set; } = new();
    public List<PendingCommentDto> PendingComments { get; set; } = [];

    public async Task OnGetAsync()
    {
        Stats = await _mediator.Send(new GetDashboardStatsQuery());
        PendingComments = await _mediator.Send(new GetPendingCommentsQuery());
    }
}
