using FluentValidation;

namespace CollegeControlSystem.Application.Courses.AddPrerequisite
{
    public sealed class AddPrerequisiteCommandValidator : AbstractValidator<AddPrerequisiteCommand>
    {
        public AddPrerequisiteCommandValidator()
        {
            RuleFor(c => c.CourseId).NotEmpty();
            RuleFor(c => c.PrerequisiteCourseId).NotEmpty()
                .NotEqual(c => c.CourseId).WithMessage("A course cannot be a prerequisite of itself.");
        }
    }
}
