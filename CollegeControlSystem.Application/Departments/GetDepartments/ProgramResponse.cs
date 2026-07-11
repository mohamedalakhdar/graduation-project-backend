namespace CollegeControlSystem.Application.Departments.GetDepartments
{
    public sealed record ProgramResponse(
        Guid Id,
        string Name,
        int RequiredCredits
    );
}
