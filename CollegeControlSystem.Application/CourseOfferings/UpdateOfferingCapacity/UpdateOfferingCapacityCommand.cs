
using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.CourseOfferings.UpdateOfferingCapacity
{
    public sealed record UpdateOfferingCapacityCommand(
        Guid OfferingId,
        int NewCapacity
    ) : ICommand;
}
