using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Registrations;

namespace CollegeControlSystem.Application.Registrations.SubmitGrades
{
    internal sealed class SubmitGradesCommandHandler : ICommandHandler<SubmitGradesCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmitGradesCommandHandler(IUnitOfWork unitOfWork, IGradeRepository gradeRepository)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(SubmitGradesCommand request, CancellationToken cancellationToken)
        {
            // 1. Verify Offering exists and belongs to this Instructor
            var offering = await _unitOfWork.CourseOfferingRepository.GetByIdAsync(request.OfferingId, cancellationToken);

            if (offering is null)
            {
                return Result.Failure(CourseOfferingErrors.OfferingNotFound);
            }

            // Security Check: Ensure the instructor grading the course is the one assigned to it
            // (Admins can bypass this check if you pass a specific flag or handle admin overrides elsewhere)
            if (offering.InstructorId != request.InstructorId)
            {
                return Result.Failure(GradeErrors.Unauthorized);
            }

            // 2. Fetch all registrations for this offering
            var courseRegistrations = await _unitOfWork.RegistrationRepository.GetByOfferingIdAsync(request.OfferingId, cancellationToken);

            // 3. Process each submitted grade
            foreach (var submission in request.Submissions)
            {
                var registration = courseRegistrations.FirstOrDefault(r => r.Id == submission.RegistrationId);

                // Skip if registration ID is invalid or student dropped/withdrew
                if (registration is null ||
                    registration.Status == RegistrationStatus.Dropped ||
                    registration.Status == RegistrationStatus.Withdrawn)
                {
                    continue;
                }

                // 4. Check if a grade already exists
                var existingGrade = await _unitOfWork.GradeRepository.GetByRegistrationIdAsync(registration.Id);

                if (existingGrade is null)
                {
                    // Create new Grade. The Domain Factory applies Retake Logic automatically if isRetake is true.
                    var gradeResult = Grade.Create(
                        registration.Id,
                        submission.SemesterWork,
                        submission.FinalExam,
                        registration.IsRetake);

                    if (gradeResult.IsFailure)
                        return Result.Failure(gradeResult.Error); // Fails on negative or >100 scores

                    _unitOfWork.GradeRepository.Add(gradeResult.Value);
                }
                else
                {
                    // Update existing Grade
                    var updateResult = existingGrade.CalculateAndSet(
                        submission.SemesterWork,
                        submission.FinalExam,
                        registration.IsRetake);

                    if (updateResult.IsFailure)
                        return Result.Failure(updateResult.Error);
                }

                // 5. Update Registration Status to Completed
                registration.MarkAsCompleted();
            }

            // 6. Save all changes atomically in a single transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
