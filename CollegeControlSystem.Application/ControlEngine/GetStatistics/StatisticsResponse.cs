namespace CollegeControlSystem.Application.ControlEngine.GetStatistics
{
    public sealed record StatisticsResponse(
        int TotalStudents,
        int ActiveStudents,
        int WarningStudents,
        int DismissedStudents,
        int GraduatedStudents,
        int TotalDepartments,
        int TotalPrograms,
        int TotalFaculties,
        int TotalCourseOfferings,
        decimal AverageCGPA,
        int ActiveRegistrations
    );
}
