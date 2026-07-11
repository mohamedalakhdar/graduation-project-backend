using FluentValidation;

namespace CollegeControlSystem.Application.Students.UpdateStudentProfile
{
    internal class UpdateStudentProfileCommandValidator:AbstractValidator<UpdateStudentProfileCommand>
    {
        public UpdateStudentProfileCommandValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student ID is required.");
            RuleFor(x => x.NewFullName)
                .NotEmpty().WithMessage("Student name cannot be empty.")
                .MaximumLength(100).WithMessage("Student name cannot exceed 100 characters.");
            // Assuming National ID must be exactly 14 digits
            RuleFor(x => x.NewNationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .Matches(@"^\d{14}$").WithMessage("National ID must be exactly 14 digits.");
        }
    }
}
