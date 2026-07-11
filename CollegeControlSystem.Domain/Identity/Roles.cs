namespace CollegeControlSystem.Domain.Identity
{
    public class Roles
    {
        public const string AdminRole = "Admin";
        public const string StudentRole = "Student";
        public const string AdvisorRole = "Advisor";
        public const string ProfessorRole = "Professor";
        public static readonly string[] All = { AdminRole, StudentRole, AdvisorRole, ProfessorRole };
    }
}
