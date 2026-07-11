using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Faculties
{
    public static class FacultyErrors
    {
        public static readonly Error DepartmentRequired = new(
            "Faculty.DepartmentRequired", "Faculty member must be assigned to a department.");

        public static readonly Error DegreeRequired = new(
            "Faculty.DegreeRequired", "Academic degree (e.g., PhD, MSc) is required.");

        public static readonly Error FacultyNameRequired = new(
            "Faculty.FacultyNameRequired", "Faculty name is required.");

        public static readonly Error SameDepartment = new(
            "Faculty.SameDepartment","Faculty is already assigned to this department.");
        public static readonly Error NotFound = new(
            "Faculty.NotFound", "Faculty member not found.");

        public static readonly Error SameStatus = new(
            "Faculty.SameStatus", "Faculty member already has this status.");
    }
}
