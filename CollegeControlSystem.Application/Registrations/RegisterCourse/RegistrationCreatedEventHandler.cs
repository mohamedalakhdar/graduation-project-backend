using CollegeControlSystem.Application.Abstractions.IService;
using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Registrations.Events;
using CollegeControlSystem.Domain.Students;
using MediatR;
namespace CollegeControlSystem.Application.Registrations.RegisterCourse
{
    internal sealed class RegistrationCreatedEventHandler : INotificationHandler<RegistrationCreatedDomainEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IFacultyRepository _facultyRepository;

        public RegistrationCreatedEventHandler(
            IEmailService emailService,
            IRegistrationRepository registrationRepository,
            IStudentRepository studentRepository,
            IFacultyRepository facultyRepository)
        {
            _emailService = emailService;
            _registrationRepository = registrationRepository;
            _studentRepository = studentRepository;
            _facultyRepository = facultyRepository;
        }

        public async Task Handle(RegistrationCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            // 1. Get the Registration to find out WHO and WHAT
            var registration = await _registrationRepository.GetByIdAsync(notification.RegistrationId, cancellationToken);
            if (registration is null) return;

            // 2. Get the Student to find out their Advisor
            var student = await _studentRepository.GetByIdAsync(registration.StudentId, cancellationToken);

            // If student not found or has no advisor, we can't email anyone. Stop.
            if (student is null || student.AdvisorId is null) return;

            // 3. Get the Advisor (Faculty) to find their Email
            // Note: Ensure your FacultyRepository includes the AppUser navigation property
            var advisor = await _facultyRepository.GetByIdAsync(student.AdvisorId.Value, cancellationToken);

            if (advisor is null || string.IsNullOrEmpty(advisor.AppUser?.Email))
            {
                // Logging: "Advisor not found or has no email"
                return;
            }

            // 4. Construct the Email
            string emailSubject = "Action Required: New Course Registration Request";

            string emailBody = $@"
            Dear Dr. {advisor.FacultyName},

            Student **{student.StudentName}** ({student.AcademicNumber}) has requested approval for a new course registration.
            
            Please log in to the College Control System dashboard to review and approve/reject this request.

            Best regards,
            College Control System
        ";

            // 5. Send Email (Fire and Forget or Await based on requirements)
            await _emailService.SendMailAsync(advisor.AppUser.Email, emailSubject, emailBody);
        }
    }
}