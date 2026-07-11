using FluentValidation;

namespace CollegeControlSystem.Application.Registrations.AppealGrade;

public sealed class AppealGradeCommandValidator : AbstractValidator<AppealGradeCommand>
{
    public AppealGradeCommandValidator()
    {
        RuleFor(x => x.RegistrationId)
            .NotEmpty()
            .WithMessage("Registration ID is required.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Appeal reason is required.")
            .MinimumLength(10)
            .WithMessage("Appeal reason must be at least 10 characters.")
            .MaximumLength(1000)
            .WithMessage("Appeal reason cannot exceed 1000 characters.");
    }
}
