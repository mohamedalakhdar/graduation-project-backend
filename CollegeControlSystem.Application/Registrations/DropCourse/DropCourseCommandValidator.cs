using FluentValidation;

namespace CollegeControlSystem.Application.Registrations.DropCourse
{
    public sealed class DropCourseCommandValidator : AbstractValidator<DropCourseCommand>
    {
        public DropCourseCommandValidator()
        {
            RuleFor(x => x.RegistrationId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
        }
    }
}
