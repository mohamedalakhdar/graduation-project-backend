using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.ChangeFacultyStatus
{
    public record ChangeFacultyStatusCommand(Guid FacultyId, FacultyStatus NewStatus) : ICommand;
}
