using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Students.GetAllStudents;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.GetStudentsByStatus
{
    internal sealed class GetStudentsByStatusQueryHandler
        : IQueryHandler<GetStudentsByStatusQuery, List<StudentListItemResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStudentsByStatusQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<StudentListItemResponse>>> Handle(
            GetStudentsByStatusQuery request,
            CancellationToken cancellationToken)
        {
            var students = await _unitOfWork.StudentRepository
                .GetByStatusAsync(request.Status, cancellationToken);

            var responses = students.Select(s => new StudentListItemResponse(
                s.Id,
                s.StudentName,
                s.AcademicNumber,
                s.Program?.Name ?? "Unassigned",
                s.CGPA,
                s.AcademicStatus.ToString(),
                s.AcademicLevel.ToString(),
                s.NationalId
            )).ToList();

            return Result<List<StudentListItemResponse>>.Success(responses);
        }
    }
}
