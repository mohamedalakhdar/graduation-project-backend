using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Faculties.GetFacultyList;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.GetFacultiesByStatus
{
    internal sealed class GetFacultiesByStatusQueryHandler
        : IQueryHandler<GetFacultiesByStatusQuery, List<GetFacultyListQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFacultiesByStatusQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<GetFacultyListQueryResponse>>> Handle(
            GetFacultiesByStatusQuery request,
            CancellationToken cancellationToken)
        {
            var faculties = await _unitOfWork.FacultyRepository
                .GetByStatusAsync(request.Status, cancellationToken);

            var responses = faculties.Select(f => new GetFacultyListQueryResponse(
                f.Id,
                f.FacultyName,
                f.Degree.ToString(),
                f.Department?.DepartmentName ?? "Unknown",
                f.AppUser?.Email ?? "Unknown",
                f.Status.ToString()
            )).ToList();

            return Result<List<GetFacultyListQueryResponse>>.Success(responses);
        }
    }
}
