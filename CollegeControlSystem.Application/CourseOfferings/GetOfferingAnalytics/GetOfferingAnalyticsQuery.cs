using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.CourseOfferings.GetOfferingAnalytics
{
    public sealed record GetOfferingAnalyticsQuery(Guid OfferingId) : IQuery<OfferingAnalyticsResponse>;
}
