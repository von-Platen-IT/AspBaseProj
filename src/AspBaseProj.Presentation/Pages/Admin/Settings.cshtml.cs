using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Features.Settings;
using AspBaseProj.Domain.Entities;
using AspBaseProj.Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspBaseProj.Presentation.Pages.Admin;

[Authorize(Policy = Policies.Root)]
public class SettingsModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly UserManager<AppUser> _userManager;

    public SettingsModel(IMediator mediator, UserManager<AppUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    [BindProperty]
    public List<SystemSettingDto> Settings { get; set; } = [];

    public async Task OnGetAsync()
    {
        Settings = await _mediator.Send(new GetSettingsQuery());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        foreach (var setting in Settings)
        {
            await _mediator.Send(new UpdateSettingCommand
            {
                Key = setting.Key,
                Value = setting.Value,
                UpdatedById = user.Id
            });
        }

        TempData["SuccessMessage"] = "Settings saved successfully.";
        return RedirectToPage();
    }
}
