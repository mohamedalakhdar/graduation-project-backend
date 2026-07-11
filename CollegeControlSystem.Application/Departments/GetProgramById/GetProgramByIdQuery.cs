using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Departments.GetProgramById
{
    public sealed record GetProgramByIdQuery(
        //Guid DepartmentId,
        Guid ProgramId
    ) : IQuery<ProgramDetailResponse>;
}
