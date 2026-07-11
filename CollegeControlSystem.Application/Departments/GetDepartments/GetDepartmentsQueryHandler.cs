using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;
using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Application.Departments.GetDepartments
{
    internal sealed class GetDepartmentsQueryHandler : IQueryHandler<GetDepartmentsQuery, PagedResponse<DepartmentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResponse<DepartmentResponse>>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _unitOfWork.DepartmentRepository.GetFilteredAsync(
                request.Search,
                request.Page,
                request.PageSize,
                cancellationToken);

            var responseItems = items.Select(dept => new DepartmentResponse(
                dept.Id,
                dept.DepartmentName,
                dept.Description,
                dept.Programs.Select(prog => new ProgramResponse(
                    prog.Id,
                    prog.Name,
                    prog.RequiredCredits
                )).ToList()
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return Result<PagedResponse<DepartmentResponse>>.Success(
                new PagedResponse<DepartmentResponse>(
                    responseItems,
                    request.Page,
                    request.PageSize,
                    totalCount,
                    totalPages));
        }
    }
}
