using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.GetAllStudents
{
    public sealed record GetAllStudentsQuery(
        string? Search,
        Guid? ProgramId,
        Guid? AdvisorId,
        AcademicStatus? Status,
        decimal? MinCGPA,
        decimal? MaxCGPA,
        int Page = 1,
        int PageSize = 10
    ) : IQuery<PagedResponse<StudentListItemResponse>>;
}
