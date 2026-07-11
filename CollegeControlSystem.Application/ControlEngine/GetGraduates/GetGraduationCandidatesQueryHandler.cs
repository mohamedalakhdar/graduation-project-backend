using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;

namespace CollegeControlSystem.Application.ControlEngine.GetGraduates
{
    internal sealed class GetGraduationCandidatesQueryHandler : IQueryHandler<GetGraduationCandidatesQuery, List<GraduationCandidateResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetGraduationCandidatesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<GraduationCandidateResponse>>> Handle(GetGraduationCandidatesQuery request, CancellationToken cancellationToken)
        {
            var candidates = await _unitOfWork.StudentRepository.GetStudentsForGraduationAuditAsync(cancellationToken);
            var responseList = new List<GraduationCandidateResponse>();

            foreach (var student in candidates)
            {
                var missingRequirements = new List<string>();

                // 1. Get all unique passed courses (If retaken, just count it once)
                var passedRegistrations = student.Registrations
                    .Where(r => r.Status == RegistrationStatus.Completed && r.Grade != null && r.Grade.IsPassing)
                    .GroupBy(r => r.CourseOffering.CourseId)
                    .Select(g => g.First())
                    .ToList();

                // 2. Calculate Total Earned Credits
                int earnedCredits = passedRegistrations.Sum(r => r.CourseOffering.Course.Credits);

                if (earnedCredits < student.Program.RequiredCredits)
                {
                    missingRequirements.Add($"Missing Credits: {student.Program.RequiredCredits - earnedCredits} hours remaining.");
                }

                // 3. Audit Article 8 & 22 Specific Course Rules
                // Note: Replace the string checks below with your actual Course Codes (e.g., Code.Value == "CSE 498")

                var gradProject1 = passedRegistrations.FirstOrDefault(r => r.CourseOffering.Course.Title.Contains("Project 1"));
                if (gradProject1 == null || gradProject1.Grade.GradePoints < 2.0m)
                {
                    missingRequirements.Add("Graduation Project 1 (Minimum 'C' required) not met.");
                }

                var gradProject2 = passedRegistrations.FirstOrDefault(r => r.CourseOffering.Course.Title.Contains("Project 2"));
                if (gradProject2 == null || gradProject2.Grade.GradePoints < 2.0m)
                {
                    missingRequirements.Add("Graduation Project 2 (Minimum 'C' required) not met.");
                }

                var summerTraining = passedRegistrations.FirstOrDefault(r => r.CourseOffering.Course.Title.Contains("Summer Training"));
                if (summerTraining == null)
                {
                    missingRequirements.Add("Summer Training (Pass required) not met.");
                }

                // 4. Build Response
                bool isEligible = !missingRequirements.Any();

                responseList.Add(new GraduationCandidateResponse(
                    student.Id,
                    student.AcademicNumber,
                    student.StudentName,
                    student.Program?.Name ?? "Unknown",
                    student.CGPA,
                    earnedCredits,
                    student.Program?.RequiredCredits ?? 0,
                    isEligible,
                    missingRequirements
                ));
            }

            // Order by eligible students first, then highest CGPA
            var sortedResponse = responseList
                .OrderByDescending(r => r.IsEligible)
                .ThenByDescending(r => r.CGPA)
                .ToList();

            return Result<List<GraduationCandidateResponse>>.Success(sortedResponse);
        }
    }
}