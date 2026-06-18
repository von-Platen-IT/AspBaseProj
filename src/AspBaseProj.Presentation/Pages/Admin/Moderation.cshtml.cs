using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Features.Comments.Commands;
using AspBaseProj.Application.Features.Comments.Queries;
using AspBaseProj.Domain.Entities;
using AspBaseProj.Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspBaseProj.Presentation.Pages.Admin;

[Authorize(Policy = Policies.Admin)]
public class ModerationModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly UserManager<AppUser> _userManager;

    public ModerationModel(IMediator mediator, UserManager<AppUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    public List<PendingCommentDto> PendingComments { get; set; } = [];

    public async Task OnGetAsync()
    {
        PendingComments = await _mediator.Send(new GetPendingCommentsQuery());
    }

    public async Task<IActionResult> OnPostApproveAsync(Guid commentId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        await _mediator.Send(new ApproveCommentCommand { CommentId = commentId, ApprovedById = user.Id });
        TempData["SuccessMessage"] = "Comment approved successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(Guid commentId)
    {
        await _mediator.Send(new RejectCommentCommand { CommentId = commentId });
        TempData["SuccessMessage"] = "Comment rejected.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostBulkActionAsync(List<Guid> commentIds, string action)
    {
        if (commentIds.Count == 0)
        {
            TempData["ErrorMessage"] = "No comments selected.";
            return RedirectToPage();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        if (action == "approve")
        {
            await _mediator.Send(new BulkApproveCommentsCommand { CommentIds = commentIds, ApprovedById = user.Id });
            TempData["SuccessMessage"] = $"{commentIds.Count} comment(s) approved.";
        }
        else if (action == "reject")
        {
            await _mediator.Send(new BulkRejectCommentsCommand { CommentIds = commentIds });
            TempData["SuccessMessage"] = $"{commentIds.Count} comment(s) rejected.";
        }

        return RedirectToPage();
    }
}
