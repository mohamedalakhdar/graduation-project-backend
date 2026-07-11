using FluentValidation;

namespace CollegeControlSystem.Application.Students.DismissStudent
{
    public sealed class DismissStudentCommandValidator : AbstractValidator<DismissStudentCommand>
    {
        public DismissStudentCommandValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty()
                .WithMessage("Student ID is required");
        }
    }
}
