using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Faculties.GetAdvisorStudents
{
    public sealed record GetAdvisorStudentsQuery(Guid AdvisorId) : IQuery<GetAdvisorStudentsQueryResponse>;
}
