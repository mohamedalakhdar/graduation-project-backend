namespace CollegeControlSystem.Presentation.Controllers.Identity
{
    public sealed record RegisterRequest(
       // Common Fields
       string UserName,
       string Email,
       string Password,
       string? PhoneNumber,
       string Role,

       // Student Specific
       string? AcademicNumber,
       string? NationalId,
       Guid? ProgramId,

       // Faculty Specific
       Guid? DepartmentId,
       string? Degree
   //bool? IsAdvisor
   );
}
