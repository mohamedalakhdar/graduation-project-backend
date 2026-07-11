namespace CollegeControlSystem.Application.Students.GetTranscript
{
    public sealed record TranscriptResponse(
        string StudentName,
        string AcademicNumber,
        string ProgramName,
        decimal CGPA,
        List<SemesterTranscriptDto> Semesters
    );

    public sealed record SemesterTranscriptDto(
        string Term,
        int Year,
        decimal SGPA,
        List<CourseTranscriptDto> Courses
    );

    public sealed record CourseTranscriptDto(
        string CourseCode,
        string CourseTitle,
        int Credits,
        string Grade, // A, B+, F
        decimal Points
    );
}
