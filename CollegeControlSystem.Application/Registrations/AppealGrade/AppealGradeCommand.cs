using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Registrations.AppealGrade;

public sealed record AppealGradeCommand(Guid RegistrationId, string Reason) : ICommand<Guid>;
