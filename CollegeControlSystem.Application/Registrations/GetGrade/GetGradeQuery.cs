using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Registrations.GetGrade;

public sealed record GetGradeQuery(Guid RegistrationId) : IQuery<GetGradeResponse>;
