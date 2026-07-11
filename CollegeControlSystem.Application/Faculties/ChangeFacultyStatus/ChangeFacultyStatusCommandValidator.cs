using CollegeControlSystem.Domain.Faculties;
using FluentValidation;

namespace CollegeControlSystem.Application.Faculties.ChangeFacultyStatus
{
    public sealed class ChangeFacultyStatusCommandValidator : AbstractValidator<ChangeFacultyStatusCommand>
    {
        public ChangeFacultyStatusCommandValidator()
        {
            RuleFor(x => x.FacultyId)
                .NotEmpty()
                .WithMessage("Faculty ID is required");

            RuleFor(x => x.NewStatus)
                .IsInEnum()
                .WithMessage("Invalid faculty status value");
        }
    }
}
