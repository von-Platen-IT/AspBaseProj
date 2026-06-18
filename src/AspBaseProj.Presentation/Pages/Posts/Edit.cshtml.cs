using System.ComponentModel.DataAnnotations;
using AspBaseProj.Application.DTOs;
using AspBaseProj.Application.Features.Posts.Commands;
using AspBaseProj.Application.Features.Posts.Queries;
using AspBaseProj.Domain.Entities;
using AspBaseProj.Presentation.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspBaseProj.Presentation.Pages.Posts;

[Authorize(Policy = Policies.AuthorOrAdmin)]
public class EditModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly UserManager<AppUser> _userManager;

    public EditModel(IMediator mediator, UserManager<AppUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool IsEdit { get; set; }
    public Guid? PostId { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required.")]
        public string ContentHtml { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Excerpt must not exceed 500 characters.")]
        public string? Excerpt { get; set; }

        public bool IsPublished { get; set; }

        public string Slug { get; set; } = string.Empty;

        public List<SocialLinkInputDto> SocialLinks { get; set; } = [];
    }

    public async Task<IActionResult> OnGetAsync(string? slug)
    {
        // Try to get slug from route data if not provided as parameter
        slug = slug ?? (string?)ViewData["slug"] ?? RouteData.Values["slug"] as string;

        if (string.IsNullOrEmpty(slug))
        {
            IsEdit = false;
            return Page();
        }

        IsEdit = true;
        PostDto post;
        try
        {
            post = await _mediator.Send(new GetPostBySlugQuery { Slug = slug });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Error loading post: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw; // Re-throw to let the global exception handler deal with it
        }

        // Check authorization: author can only edit own posts
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Challenge();

        if (!user.IsRoot && post.AuthorId != user.Id)
            return Forbid();

        PostId = post.Id;
        Input = new InputModel
        {
            Title = post.Title,
            ContentHtml = post.ContentHtml,
            Excerpt = post.Excerpt,
            IsPublished = post.IsPublished,
            Slug = post.Slug,
            SocialLinks = post.SocialLinks.Select(s => new SocialLinkInputDto
            {
                Url = s.Url,
                Platform = s.Platform,
                Title = s.Title,
                DisplayOrder = s.DisplayOrder
            }).ToList()
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? slug)
    {
        if (!ModelState.IsValid)
        {
            IsEdit = !string.IsNullOrEmpty(slug);
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Challenge();

        if (string.IsNullOrEmpty(slug))
        {
            // Create new post
            var post = await _mediator.Send(new CreatePostCommand
            {
                AuthorId = user.Id,
                Title = Input.Title,
                ContentHtml = Input.ContentHtml,
                Excerpt = Input.Excerpt,
                IsPublished = Input.IsPublished,
                SocialLinks = Input.SocialLinks
            });

            TempData["SuccessMessage"] = "Post created successfully!";
            return RedirectToPage("/Posts/Detail", new { slug = post.Slug });
        }
        else
        {
            // Update existing post
            var existingPost = await _mediator.Send(new GetPostBySlugQuery { Slug = slug });
            await _mediator.Send(new UpdatePostCommand
            {
                PostId = existingPost.Id,
                CurrentUserId = user.Id,
                IsCurrentUserRoot = user.IsRoot,
                Title = Input.Title,
                ContentHtml = Input.ContentHtml,
                Excerpt = Input.Excerpt,
                IsPublished = Input.IsPublished,
                SocialLinks = Input.SocialLinks
            });

            TempData["SuccessMessage"] = "Post updated successfully!";
            return RedirectToPage("/Posts/Detail", new { slug = slug });
        }
    }
}
