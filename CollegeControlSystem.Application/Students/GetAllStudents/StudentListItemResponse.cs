namespace CollegeControlSystem.Application.Students.GetAllStudents
{
    public sealed record StudentListItemResponse(
        Guid Id,
        string FullName,
        string AcademicNumber,
        string ProgramName,
        decimal CGPA,
        string AcademicStatus,
        string AcademicLevel,
        string NationalId
    );
}
