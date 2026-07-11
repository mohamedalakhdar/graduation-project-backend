namespace CollegeControlSystem.Application.Departments.GetProgramById
{
    public sealed record ProgramDetailResponse(
        Guid Id,
        string Name,
        int RequiredCredits,
        Guid DepartmentId,
        string DepartmentName
    );
}
