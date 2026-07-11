using FluentValidation;

namespace CollegeControlSystem.Application.RegistrationPeriods.CreateRegistrationPeriod
{
    public sealed class CreateRegistrationPeriodCommandValidator
        : AbstractValidator<CreateRegistrationPeriodCommand>
    {
        public CreateRegistrationPeriodCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Name must be between 1 and 200 characters.");

            RuleFor(x => x.StartDateUtc)
                .NotEmpty()
                .WithMessage("Start date is required.");

            RuleFor(x => x.EndDateUtc)
                .NotEmpty()
                .WithMessage("End date is required.")
                .GreaterThan(x => x.StartDateUtc)
                .WithMessage("End date must be after start date.");

            RuleFor(x => x.Term)
                .NotEmpty()
                .Must(t => t is "Fall" or "Spring" or "Summer")
                .WithMessage("Term must be Fall, Spring, or Summer.");

            RuleFor(x => x.Year)
                .InclusiveBetween(2020, 2100)
                .WithMessage("Year must be between 2020 and 2100.");
        }
    }
}
