using FluentValidation;

namespace CollegeControlSystem.Application.Courses.UpdateCourse;

public sealed class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(c => c.CourseId).NotEmpty();
        RuleFor(c => c.DepartmentId).NotEmpty();
        RuleFor(c => c.Title).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Description).MaximumLength(1000);
        RuleFor(c => c.Credits).GreaterThan(0);
        RuleFor(c => c.LectureHours).GreaterThanOrEqualTo(0);
        RuleFor(c => c.LabHours).GreaterThanOrEqualTo(0);
    }
}
