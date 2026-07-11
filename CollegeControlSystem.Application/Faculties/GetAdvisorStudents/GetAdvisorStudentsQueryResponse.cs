namespace CollegeControlSystem.Application.Faculties.GetAdvisorStudents
{
    public sealed class GetAdvisorStudentsQueryResponse
    {
       public  Guid AdvisorId { get; set; }
        public  List<AdvisorStudentResponse> advisorStudentResponses { get; set; }
    }
    public sealed record AdvisorStudentResponse
    (
        Guid StudentId,
        string Name,
        string AcademicNumber,
        string ProgramName,
        decimal CGPA,
        string AcademicStatus
    );
}
