using FluentValidation;

namespace CollegeControlSystem.Application.CourseOfferings.UpdateCourseOffering;

public sealed class UpdateCourseOfferingCommandValidator : AbstractValidator<UpdateCourseOfferingCommand>
{
    public UpdateCourseOfferingCommandValidator()
    {
        RuleFor(x => x.OfferingId).NotEmpty();
        RuleFor(x => x.NewCapacity).GreaterThan(0);
        RuleFor(x => x.NewInstructorId).NotEmpty();
    }
}
