using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Departments.UpdateProgramCredits
{
    public sealed record UpdateProgramCreditsCommand(
        //Guid DepartmentId,
        Guid ProgramId,
        int NewRequiredCredits
    ) : ICommand;

}
