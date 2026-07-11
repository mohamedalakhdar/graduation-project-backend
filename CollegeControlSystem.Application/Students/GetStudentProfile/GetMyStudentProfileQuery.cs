using CollegeControlSystem.Application.Abstractions.Messaging;
using System.Security.Claims;

namespace CollegeControlSystem.Application.Students.GetStudentProfile
{
    public record GetMyStudentProfileQuery(ClaimsPrincipal User) : IQuery<StudentResponse>;
}
