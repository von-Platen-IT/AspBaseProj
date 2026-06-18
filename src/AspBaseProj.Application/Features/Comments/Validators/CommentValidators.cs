using FluentValidation;

namespace AspBaseProj.Application.Features.Comments.Validators;

/// <summary>
/// Validator for <see cref="Commands.AddCommentCommand"/>.
/// </summary>
public sealed class AddCommentCommandValidator : AbstractValidator<Commands.AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("Post ID is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(5000).WithMessage("Comment must not exceed 5000 characters.");

        // Guest fields required when UserId is null
        When(x => x.UserId is null, () =>
        {
            RuleFor(x => x.GuestName)
                .NotEmpty().WithMessage("Guest name is required for unauthenticated users.")
                .MaximumLength(100).WithMessage("Guest name must not exceed 100 characters.");

            RuleFor(x => x.GuestEmail)
                .NotEmpty().WithMessage("Guest email is required for unauthenticated users.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(200).WithMessage("Email must not exceed 200 characters.");
        });
    }
}
