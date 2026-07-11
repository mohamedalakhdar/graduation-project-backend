using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;

namespace CollegeControlSystem.Application.CourseOfferings.GetAvailableOfferings
{
    public sealed record GetAvailableOfferingsQuery(
        string? Term,
        int? Year,
        Guid? CourseId,
        Guid? InstructorId,
        int Page = 1,
        int PageSize = 10
    ) : IQuery<PagedResponse<OfferingQueryResponse>>;
}
