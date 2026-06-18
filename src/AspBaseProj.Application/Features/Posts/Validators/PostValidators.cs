using FluentValidation;

namespace AspBaseProj.Application.Features.Posts.Validators;

/// <summary>
/// Validator for <see cref="Commands.CreatePostCommand"/>.
/// </summary>
public sealed class CreatePostCommandValidator : AbstractValidator<Commands.CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.ContentHtml)
            .NotEmpty().WithMessage("Content is required.");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author ID is required.");

        RuleFor(x => x.Excerpt)
            .MaximumLength(500).WithMessage("Excerpt must not exceed 500 characters.");

        RuleForEach(x => x.SocialLinks).ChildRules(social =>
        {
            social.RuleFor(s => s.Url)
                .NotEmpty().WithMessage("Social link URL is required.")
                .MaximumLength(2000).WithMessage("URL must not exceed 2000 characters.");
        });
    }
}

/// <summary>
/// Validator for <see cref="Commands.UpdatePostCommand"/>.
/// </summary>
public sealed class UpdatePostCommandValidator : AbstractValidator<Commands.UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("Post ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.ContentHtml)
            .NotEmpty().WithMessage("Content is required.");

        RuleFor(x => x.Excerpt)
            .MaximumLength(500).WithMessage("Excerpt must not exceed 500 characters.");

        RuleForEach(x => x.SocialLinks).ChildRules(social =>
        {
            social.RuleFor(s => s.Url)
                .NotEmpty().WithMessage("Social link URL is required.")
                .MaximumLength(2000).WithMessage("URL must not exceed 2000 characters.");
        });
    }
}
