using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Application.ControlEngine.GetWarnings
{
    internal sealed class GetAcademicWarningsQueryHandler : IQueryHandler<GetAcademicWarningsQuery, List<AcademicWarningResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAcademicWarningsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<AcademicWarningResponse>>> Handle(GetAcademicWarningsQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch data using the specific repository method
            var students = await _unitOfWork.StudentRepository.GetStudentsWithWarningsAsync(cancellationToken);

            // 2. Map to DTO
            var response = students.Select(s => new AcademicWarningResponse(
                s.Id,
                s.AcademicNumber,
                s.StudentName,
                s.Program?.Name ?? "Unknown Program",
                s.CGPA,
                s.ConsecutiveWarnings,
                s.AcademicStatus.ToString() // "AcademicWarning" or "Dismissed"
            )).ToList();

            // 3. Return success
            return Result<List<AcademicWarningResponse>>.Success(response);
        }
    }
}
