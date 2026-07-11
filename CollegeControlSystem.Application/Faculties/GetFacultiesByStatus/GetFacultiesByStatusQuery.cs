using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Faculties.GetFacultyList;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.GetFacultiesByStatus
{
    public record GetFacultiesByStatusQuery(FacultyStatus Status)
        : IQuery<List<GetFacultyListQueryResponse>>;
}
