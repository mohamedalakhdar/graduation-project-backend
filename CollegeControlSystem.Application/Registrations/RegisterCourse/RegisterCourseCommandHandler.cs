using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.RegistrationPeriods;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Registrations.Events;
using CollegeControlSystem.Domain.Students;
using MediatR;

namespace CollegeControlSystem.Application.Registrations.RegisterCourse
{
    internal sealed class RegisterCourseCommandHandler : ICommandHandler<RegisterCourseCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public RegisterCourseCommandHandler(
            IUnitOfWork unitOfWork,
            IMediator mediator
            )
        {
            _mediator=mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(RegisterCourseCommand request, CancellationToken cancellationToken)
        {
            // 1. Load Data
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student is null) return Result<Guid>.Failure(StudentErrors.StudentNotFound);

            var offering = await _unitOfWork.CourseOfferingRepository.GetByIdAsync(request.CourseOfferingId, cancellationToken);
            if (offering is null) return Result<Guid>.Failure(CourseOfferingErrors.OfferingNotFound);

            if (offering.IsCancelled)
                return Result<Guid>.Failure(CourseOfferingErrors.OfferingCancelled);

            // 1.5 Check: Is registration currently open?
            var activePeriod = await _unitOfWork.RegistrationPeriodRepository
                .GetCurrentActivePeriodAsync(cancellationToken);
            if (activePeriod is null || !activePeriod.IsOpenForRegistration())
                return Result<Guid>.Failure(RegistrationPeriodErrors.RegistrationClosed);

            // 2. Check: Is student already registered?
            var existingRegistration = await _unitOfWork.RegistrationRepository.GetByStudentAndOfferingAsync(request.StudentId, request.CourseOfferingId, cancellationToken);
            if (existingRegistration is not null)
            {
                return Result<Guid>.Failure(RegistrationErrors.DuplicateRegistration);
            }

            // 3. Check: Prerequisites
            // We need to fetch the student's history for the required prerequisite courses.
            // Assuming offering.Course includes Prerequisites list.
            foreach (var prereq in offering.Course.Prerequisites)
            {
                var history = await _unitOfWork.RegistrationRepository.GetByStudentAndCourseIdAsync(request.StudentId, prereq.PrerequisiteCourseId, cancellationToken);

                // Check if they passed any of the previous attempts
                bool hasPassed = history.Any(r => r.Grade != null && r.Grade.IsPassing); // Accessing Grade Value Object Logic

                if (!hasPassed)
                {
                    return Result<Guid>.Failure(RegistrationErrors.PrerequisiteNotMet(prereq.PrerequisiteCourseId));
                }
            }

            // 4. Check: Academic Load Limit (Article 12)
            // We need to calculate how many credits they are *currently* registered for this semester.
            // This query might need to be added to RegistrationRepository.
            // var currentLoad = await _registrationRepository.GetTotalCreditsForSemesterAsync(student.Id, offering.Semester); 
            // For now, assuming currentLoad is 0 for simplicity or fetched similarly.
            int currentLoad = 0;

            int maxAllowed = student.GetMaxAllowedCreditHours(isGraduatingSemester: false); // Using Student Domain Logic

            if (currentLoad + offering.Course.Credits > maxAllowed)
            {
                return Result<Guid>.Failure(RegistrationErrors.Overload(maxAllowed));
            }

            // 5. Check: Is it a Retake?
            // Fetch history for THIS course
            var courseHistory = await _unitOfWork.RegistrationRepository.GetByStudentAndCourseIdAsync(request.StudentId, offering.CourseId, cancellationToken);
            bool isRetake = courseHistory.Any();

            // 6. Check: Capacity (Using Domain Logic)
            var reserveResult = offering.ReserveSeat();
            if (reserveResult.IsFailure)
            {
                return Result<Guid>.Failure(reserveResult.Error);
            }

            // 7. Create Registration
            var registrationResult = Registration.Create(
                student.Id,
                offering.Id,
                isRetake
            );

            if (registrationResult.IsFailure) return Result<Guid>.Failure(registrationResult.Error);

            // 8. Save
            _unitOfWork.RegistrationRepository.Add(registrationResult.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // (Optional) Trigger Event for Email Notification
             //await _mediator.Publish(new RegistrationCreatedDomainEvent(registrationResult.Value.Id));

            return Result<Guid>.Success(registrationResult.Value.Id);
        }
    }
}
