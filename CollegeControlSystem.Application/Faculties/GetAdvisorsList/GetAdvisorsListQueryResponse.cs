namespace CollegeControlSystem.Application.Faculties.GetAdvisorsList
{
    public sealed record GetAdvisorsListQueryResponse(
        Guid Id,
        string Name,
        string Degree,
        string DepartmentName,
        string Email
    );
}
