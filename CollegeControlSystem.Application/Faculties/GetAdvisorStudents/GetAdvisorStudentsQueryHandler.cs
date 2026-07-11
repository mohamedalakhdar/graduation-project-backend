using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Students;
namespace CollegeControlSystem.Application.Faculties.GetAdvisorStudents
{
    internal sealed class GetAdvisorStudentsQueryHandler : IQueryHandler<GetAdvisorStudentsQuery, GetAdvisorStudentsQueryResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetAdvisorStudentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<Result<GetAdvisorStudentsQueryResponse>> Handle(GetAdvisorStudentsQuery request, CancellationToken cancellationToken)
        {
            var students = await _uow.StudentRepository.GetByAdvisorIdAsync(request.AdvisorId, cancellationToken);

            var advisorStudents = students.Select(s => new AdvisorStudentResponse(
                s.Id,
                s.StudentName,
                s.AcademicNumber,
                s.Program?.Name ?? "N/A",
                s.CGPA,
                s.AcademicStatus.ToString()
            )).ToList();

            var response = new GetAdvisorStudentsQueryResponse
            {
                AdvisorId = request.AdvisorId,
                advisorStudentResponses = advisorStudents
            };

            return Result<GetAdvisorStudentsQueryResponse>.Success(response);
        }
    }
}
