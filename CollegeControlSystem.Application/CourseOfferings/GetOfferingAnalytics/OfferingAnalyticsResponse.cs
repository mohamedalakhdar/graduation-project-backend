namespace CollegeControlSystem.Application.CourseOfferings.GetOfferingAnalytics
{
    public sealed record OfferingAnalyticsResponse(
        Guid OfferingId,
        string CourseCode,
        string CourseTitle,
        string Semester,
        string InstructorName,
        int Capacity,
        int Enrolled,
        double FillRate,
        int RetakeCount,
        int WithdrawnCount,
        GradeDistributionDto GradeDistribution,
        double PassRate,
        double AverageGradePoints
    );

    public sealed record GradeDistributionDto(
        int TotalGraded,
        Dictionary<string, int> LetterGradeCounts
    );
}
