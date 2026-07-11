using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;
using CollegeControlSystem.Domain.Registrations;

namespace CollegeControlSystem.Application.Registrations.GetAllRegistrations;

public sealed record GetAllRegistrationsQuery(
    RegistrationStatus? Status,
    Guid? StudentId,
    Guid? CourseOfferingId,
    string? Semester,
    int? Year,
    int Page = 1,
    int PageSize = 10
) : IQuery<PagedResponse<RegistrationResponse>>;
