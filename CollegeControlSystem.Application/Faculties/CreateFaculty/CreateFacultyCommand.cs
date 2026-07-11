using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.CreateFaculty
{
    public sealed record CreateFacultyCommand(
    string UserName,
    string Email,
    string Password,
    string? PhoneNumber,
    string FullName,
    Guid DepartmentId,
    FacultyDegree Degree,
    bool IsAdvisor
    ) : ICommand<Guid>;

}
