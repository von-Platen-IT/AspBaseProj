using System.ComponentModel.DataAnnotations;
using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Features.Posts.Queries;
using AspBaseProj.Application.Features.Users;
using AspBaseProj.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspBaseProj.Presentation.Pages.Account;

public class ProfileModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly UserManager<AppUser> _userManager;

    public ProfileModel(IMediator mediator, UserManager<AppUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    public UserDto Profile { get; set; } = null!;
    public List<PostSummaryDto> MyPosts { get; set; } = [];

    [BindProperty]
    public EditProfileInput EditInput { get; set; } = new();

    public class EditProfileInput
    {
        [Display(Name = "Display Name")]
        [StringLength(100, ErrorMessage = "Display name must not exceed 100 characters.")]
        public string? DisplayName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        Profile = await _mediator.Send(new GetUserByIdQuery { UserId = user.Id });
        MyPosts = await _mediator.Send(new GetPostsByAuthorQuery { AuthorId = user.Id, IncludeUnpublished = true });

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateProfileAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        if (!ModelState.IsValid)
        {
            Profile = await _mediator.Send(new GetUserByIdQuery { UserId = user.Id });
            MyPosts = await _mediator.Send(new GetPostsByAuthorQuery { AuthorId = user.Id, IncludeUnpublished = true });
            return Page();
        }

        await _mediator.Send(new UpdateProfileCommand
        {
            UserId = user.Id,
            DisplayName = EditInput.DisplayName,
            Email = EditInput.Email
        });

        TempData["SuccessMessage"] = "Profile updated successfully.";
        return RedirectToPage();
    }
}
