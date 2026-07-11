using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.GetAllStudents
{
    internal sealed class GetAllStudentsQueryHandler : IQueryHandler<GetAllStudentsQuery, PagedResponse<StudentListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetAllStudentsQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<PagedResponse<StudentListItemResponse>>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _uow.StudentRepository.GetFilteredAsync(
                request.Search,
                request.ProgramId,
                request.AdvisorId,
                request.Status,
                request.MinCGPA,
                request.MaxCGPA,
                request.Page,
                request.PageSize,
                cancellationToken);

            var responseItems = items.Select(s => new StudentListItemResponse(
                s.Id,
                s.StudentName,
                s.AcademicNumber,
                s.Program?.Name ?? "Unassigned",
                s.CGPA,
                s.AcademicStatus.ToString(),
                s.AcademicLevel.ToString(),
                s.NationalId
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return Result<PagedResponse<StudentListItemResponse>>.Success(
                new PagedResponse<StudentListItemResponse>(
                    responseItems,
                    request.Page,
                    request.PageSize,
                    totalCount,
                    totalPages));
        }
    }
}
