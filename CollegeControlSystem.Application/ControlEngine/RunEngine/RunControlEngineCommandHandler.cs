using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Shared;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.ControlEngine.RunEngine
{
    internal sealed class RunControlEngineCommandHandler : ICommandHandler<RunControlEngineCommand, ControlEngineSummaryResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RunControlEngineCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ControlEngineSummaryResponse>> Handle(RunControlEngineCommand request, CancellationToken cancellationToken)
        {
            // 1. Create target Semester Value Object
            var semesterResult = Semester.Create(request.Term, request.Year);
            if (semesterResult.IsFailure) return Result<ControlEngineSummaryResponse>.Failure(semesterResult.Error);
            var targetSemester = semesterResult.Value;

            // 2. Fetch all active students and their transcripts
            var students = await _unitOfWork.StudentRepository.GetActiveStudentsWithTranscriptsAsync(cancellationToken);

            int totalProcessed = 0;
            int newWarnings = 0;
            int dismissals = 0;
            int recovered = 0;

            // 3. Process each student
            foreach (var student in students)
            {
                var previousStatus = student.AcademicStatus;

                // Execute Math
                var gpaResult = GpaCalculatorService.Calculate(student.Registrations, targetSemester);

                // Skip processing warnings for students who didn't take any courses this specific semester (e.g., Leave of Absence)
                if (!gpaResult.IsActiveThisSemester) continue;

                // Update Domain Entity (This handles Warnings, Dismissals, and Academic Level internally)
                student.ProcessSemesterResults(gpaResult.SGPA, gpaResult.CGPA, gpaResult.EarnedCredits);

                // Analytics tracking for the Response Summary
                if (student.AcademicStatus == AcademicStatus.Dismissed && previousStatus != AcademicStatus.Dismissed)
                    dismissals++;
                else if (student.AcademicStatus == AcademicStatus.AcademicWarning && previousStatus == AcademicStatus.GoodStanding)
                    newWarnings++;
                else if (student.AcademicStatus == AcademicStatus.GoodStanding && previousStatus == AcademicStatus.AcademicWarning)
                    recovered++;

                totalProcessed++;
            }

            // 4. Commit all changes atomically
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ControlEngineSummaryResponse>.Success(new ControlEngineSummaryResponse(
                TotalProcessed: totalProcessed,
                NewWarningsIssued: newWarnings,
                StudentsDismissed: dismissals,
                StudentsReturnedToGoodStanding: recovered
            ));
        }
    }
}
