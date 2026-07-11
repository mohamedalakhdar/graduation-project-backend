using CollegeControlSystem.Domain.Registrations;
using FluentValidation;

namespace CollegeControlSystem.Application.Registrations.ReviewAppeal;

public sealed class ReviewAppealCommandValidator : AbstractValidator<ReviewAppealCommand>
{
    public ReviewAppealCommandValidator()
    {
        RuleFor(x => x.AppealId)
            .NotEmpty()
            .WithMessage("Appeal ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Valid appeal status is required (Approved or Rejected).");

        RuleFor(x => x.ReviewNotes)
            .NotEmpty()
            .WithMessage("Review notes are required.")
            .MaximumLength(2000)
            .WithMessage("Review notes cannot exceed 2000 characters.");

        RuleFor(x => x.ReviewedBy)
            .NotEmpty()
            .WithMessage("Reviewer ID is required.");

        RuleFor(x => x.Status)
            .NotEqual(GradeAppealStatus.Pending)
            .WithMessage("Appeal status must be Approved or Rejected.");
    }
}
