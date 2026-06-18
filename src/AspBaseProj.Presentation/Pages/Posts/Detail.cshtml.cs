using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Features.Comments.Commands;
using AspBaseProj.Application.Features.Comments.Queries;
using AspBaseProj.Application.Features.Posts.Commands;
using AspBaseProj.Application.Features.Posts.Queries;
using AspBaseProj.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspBaseProj.Presentation.Pages.Posts;

public class DetailModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly UserManager<AppUser> _userManager;

    public DetailModel(IMediator mediator, UserManager<AppUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    public PostDto Post { get; set; } = null!;
    public List<CommentDto> Comments { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        try
        {
            Post = await _mediator.Send(new GetPostBySlugQuery { Slug = slug });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        // Increment view count (fire and forget style, but await for correctness)
        _ = _mediator.Send(new IncrementViewCountCommand { PostId = Post.Id });

        Comments = await _mediator.Send(new GetCommentsByPostQuery { PostId = Post.Id });

        return Page();
    }

    [BindProperty]
    public string? PostSlug { get; set; }

    public async Task<IActionResult> OnPostAddCommentAsync(Guid postId, Guid? parentCommentId, string content, string? guestName, string? guestEmail, string postSlug)
    {
        Guid? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            userId = user?.Id;
        }

        await _mediator.Send(new AddCommentCommand
        {
            PostId = postId,
            ParentCommentId = parentCommentId,
            UserId = userId,
            GuestName = guestName,
            GuestEmail = guestEmail,
            Content = content
        });

        TempData["SuccessMessage"] = User.Identity?.IsAuthenticated == true
            ? "Comment posted successfully!"
            : "Comment submitted for moderation. It will appear after approval.";

        return RedirectToPage("/Posts/Detail", new { slug = postSlug });
    }

    public async Task<IActionResult> OnPostDeletePostAsync(Guid postId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Challenge();

        var isRoot = user.IsRoot;

        try
        {
            await _mediator.Send(new DeletePostCommand
            {
                PostId = postId,
                CurrentUserId = user.Id,
                IsCurrentUserRoot = isRoot
            });

            TempData["SuccessMessage"] = "Post deleted successfully.";
            return RedirectToPage("/Index");
        }
        catch (UnauthorizedAccessException)
        {
            TempData["ErrorMessage"] = "You are not authorized to delete this post.";
            return RedirectToPage("/Index");
        }
    }

}
