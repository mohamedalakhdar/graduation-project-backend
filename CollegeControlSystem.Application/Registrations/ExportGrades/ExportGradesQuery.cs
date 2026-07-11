using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Registrations.ExportGrades
{
    public sealed record ExportGradesQuery(
            Guid OfferingId,
            Guid RequestingUserId,
            bool IsAdmin
        ) : IQuery<FileResponse>;
}
