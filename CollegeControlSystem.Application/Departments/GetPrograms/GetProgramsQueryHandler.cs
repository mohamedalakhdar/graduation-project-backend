using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;
using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Application.Departments.GetPrograms
{
    internal sealed class GetProgramsQueryHandler : IQueryHandler<GetProgramsQuery, PagedResponse<ProgramListResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProgramsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResponse<ProgramListResponse>>> Handle(GetProgramsQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _unitOfWork.DepartmentRepository.GetProgramsFilteredAsync(
                request.Search,
                request.DepartmentId,
                request.Page,
                request.PageSize,
                cancellationToken);

            var responseItems = items.Select(prog => new ProgramListResponse(
                prog.Id,
                prog.Name,
                prog.Department.DepartmentName,
                prog.RequiredCredits
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return Result<PagedResponse<ProgramListResponse>>.Success(
                new PagedResponse<ProgramListResponse>(
                    responseItems,
                    request.Page,
                    request.PageSize,
                    totalCount,
                    totalPages));
        }
    }
}
