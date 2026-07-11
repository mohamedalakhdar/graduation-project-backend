namespace CollegeControlSystem.Application.Departments.GetPrograms
{
    public sealed record ProgramListResponse(
        Guid Id,
        string Name,
        string DepartmentName, // Helpful UI context
        int RequiredCredits
    );
}
