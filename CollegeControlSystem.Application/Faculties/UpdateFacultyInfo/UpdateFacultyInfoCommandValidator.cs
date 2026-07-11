using FluentValidation;

namespace CollegeControlSystem.Application.Faculties.UpdateFacultyInfo
{
    public sealed class UpdateFacultyInfoCommandValidator : AbstractValidator<UpdateFacultyInfoCommand>
    {
        public UpdateFacultyInfoCommandValidator()
        {
            RuleFor(x => x.FacultyId)
                .NotEmpty()
                .WithMessage("FacultyId cannot be empty.");

            RuleFor(x => x.NewDegree)
                .NotEmpty().WithMessage("Degree cannot be empty.")
                .IsInEnum().WithMessage("Degree must be a valid enum value.");
        }
    }
}
