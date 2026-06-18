using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Features.Users;
using AspBaseProj.Domain.Enums;
using AspBaseProj.Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspBaseProj.Presentation.Pages.Admin;

[Authorize(Policy = Policies.Root)]
public class UsersModel : PageModel
{
    private readonly IMediator _mediator;

    public UsersModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public List<UserDto> Users { get; set; } = [];

    public async Task OnGetAsync()
    {
        Users = await _mediator.Send(new GetUsersQuery());
    }

    public async Task<IActionResult> OnPostChangeGroupAsync(Guid userId, UserGroup group)
    {
        try
        {
            await _mediator.Send(new ChangeUserGroupCommand { UserId = userId, Group = group });
            TempData["SuccessMessage"] = $"User group changed to {group}.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(Guid userId, bool isActive)
    {
        try
        {
            await _mediator.Send(new UpdateUserStatusCommand { UserId = userId, IsActive = isActive });
            TempData["SuccessMessage"] = isActive ? "User activated." : "User deactivated.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }
}
