using FluentValidation;
namespace CollegeControlSystem.Application.Courses.CreateCourse
{
    public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(c => c.DepartmentId).NotEmpty();
            RuleFor(c => c.Title).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Code).NotEmpty().Matches(@"^[A-Z]{3}\s?[0-9]{3}$")
                .WithMessage("Course code must follow format 'ABC 123'.");
            RuleFor(c => c.Credits).GreaterThan(0);
            RuleFor(c => c.LectureHours).GreaterThanOrEqualTo(0);
            RuleFor(c => c.LabHours).GreaterThanOrEqualTo(0);
        }
    }
}
