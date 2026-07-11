using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.UpdateFacultyInfo
{
    public sealed record UpdateFacultyInfoCommand(
        Guid FacultyId,
        FacultyDegree NewDegree
    ) : ICommand;
}
