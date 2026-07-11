using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Students.GetAllStudents;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.GetStudentsByStatus
{
    public record GetStudentsByStatusQuery(AcademicStatus Status)
        : IQuery<List<StudentListItemResponse>>;
}
