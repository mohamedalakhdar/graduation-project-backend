using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.CourseOfferings.GetAvailableOfferings;

namespace CollegeControlSystem.Application.CourseOfferings.GetOfferingById;

public sealed record GetOfferingByIdQuery(Guid OfferingId) : IQuery<OfferingQueryResponse>;
