using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Faculties.TransferDepartment
{
    public sealed record TransferDepartmentCommand(
        Guid FacultyId,
        Guid NewDepartmentId
    ) : ICommand;
}
