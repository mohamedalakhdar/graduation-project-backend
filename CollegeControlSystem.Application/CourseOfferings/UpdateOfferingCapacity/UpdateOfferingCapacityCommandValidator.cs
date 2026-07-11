using FluentValidation;
namespace CollegeControlSystem.Application.CourseOfferings.UpdateOfferingCapacity
{
    public sealed class UpdateOfferingCapacityCommandValidator : AbstractValidator<UpdateOfferingCapacityCommand>
    {
        public UpdateOfferingCapacityCommandValidator()
        {
            RuleFor(x => x.OfferingId).NotEmpty();
            RuleFor(x => x.NewCapacity).GreaterThan(0);
        }
    }
}
