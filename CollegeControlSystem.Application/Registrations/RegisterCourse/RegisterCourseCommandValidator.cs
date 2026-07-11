using FluentValidation;

namespace CollegeControlSystem.Application.Registrations.RegisterCourse
{
    public sealed class RegisterCourseCommandValidator : AbstractValidator<RegisterCourseCommand>
    {
        public RegisterCourseCommandValidator()
        {
            RuleFor(c => c.StudentId).NotEmpty();
            RuleFor(c => c.CourseOfferingId).NotEmpty();
        }
    }
}
