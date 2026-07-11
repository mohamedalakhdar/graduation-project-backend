using FluentValidation;
namespace CollegeControlSystem.Application.CourseOfferings.CreateCourseOffering
{
    public sealed class CreateCourseOfferingCommandValidator : AbstractValidator<CreateCourseOfferingCommand>
    {
        public CreateCourseOfferingCommandValidator()
        {
            RuleFor(x => x.CourseId).NotEmpty();
            RuleFor(x => x.InstructorId).NotEmpty();
            RuleFor(x => x.Year).GreaterThanOrEqualTo(DateTime.UtcNow.Year);
            RuleFor(x => x.Capacity).GreaterThan(0);
            RuleFor(x => x.Term).NotEmpty();
        }
    }
}
