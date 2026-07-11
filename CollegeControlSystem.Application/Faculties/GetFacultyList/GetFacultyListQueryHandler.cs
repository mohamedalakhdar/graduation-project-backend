using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.GetFacultyList
{
    internal sealed class GetFacultyListQueryHandler : IQueryHandler<GetFacultyListQuery, PagedResponse<GetFacultyListQueryResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetFacultyListQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<PagedResponse<GetFacultyListQueryResponse>>> Handle(GetFacultyListQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _uow.FacultyRepository.GetFilteredAsync(
                request.Search,
                request.DepartmentId,
                request.Status,
                request.Page,
                request.PageSize,
                cancellationToken);

            var responseItems = items.Select(f => new GetFacultyListQueryResponse(
                f.Id,
                f.FacultyName,
                f.Degree.ToString(),
                f.Department?.DepartmentName ?? "Unknown",
                f.AppUser?.Email ?? "Unknown",
                f.Status.ToString()
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return Result<PagedResponse<GetFacultyListQueryResponse>>.Success(
                new PagedResponse<GetFacultyListQueryResponse>(
                    responseItems,
                    request.Page,
                    request.PageSize,
                    totalCount,
                    totalPages));
        }
    }
}
