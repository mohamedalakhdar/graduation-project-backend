namespace CollegeControlSystem.Application.Courses.GetCourseDetails
{
    public sealed record CourseDetailsQueryResponse(
    Guid Id,
    string Code,
    string Title,
    string Description,
    int Credits,
    int LectureHours,
    int LabHours,
    string DepartmentId, // Or DepartmentName if you include it
    List<PrerequisiteResponse> Prerequisites
);

    public sealed record PrerequisiteResponse(Guid CourseId, string Code, string Title);
}
