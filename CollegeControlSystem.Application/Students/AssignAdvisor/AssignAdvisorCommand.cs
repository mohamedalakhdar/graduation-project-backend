using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Students.AssignAdvisor
{
    public sealed record AssignAdvisorCommand(Guid StudentId, Guid AdvisorId) : ICommand;
}
