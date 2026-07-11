namespace CollegeControlSystem.Application.Departments.GetDepartments
{
    public sealed record DepartmentResponse(
        Guid Id,
        string Name,
        string? Description,
        List<ProgramResponse> Programs
    );
}
