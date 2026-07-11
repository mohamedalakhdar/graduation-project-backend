namespace CollegeControlSystem.Application.Faculties.GetFacultyList
{
    public sealed record GetFacultyListQueryResponse(
        Guid Id,
        string Name,
        string Degree,
        string DepartmentName,
        string Email,
        string Status
    );
}
