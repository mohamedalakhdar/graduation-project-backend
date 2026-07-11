using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Students.GetStudentProfile
{
    public record GetStudentProfileQuery(Guid StudentId) : IQuery<StudentResponse>;
}
