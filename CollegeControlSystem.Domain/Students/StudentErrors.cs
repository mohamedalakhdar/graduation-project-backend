using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Students
{
    public static class StudentErrors
    {
        public static readonly Error NationalIdInvalid = new ( "Student.InvalideNationalId","National ID must be 14 digits.");
        public static readonly Error AcademicNumberRequired = new(
        "Student.AcademicNumberRequired", "Academic number is required.");

        public static readonly Error NationalIdRequired = new(
            "Student.NationalIdRequired", "National ID is required.");

        public static readonly Error ProgramRequired = new(
            "Student.ProgramRequired", "Student must be assigned to a program.");

        public static readonly Error InvalidAdvisor = new(
            "Student.InvalidAdvisor", "Cannot assign an empty advisor ID.");

        public static readonly Error DuplicateAcademicNumber = new(
            "Student.DuplicateAcademicNumber", "A student with this Academic Number already exists.");
        public static readonly Error StudentNotFound = new(
            "Student.NotFound", "Student not found.");

        public static readonly Error AlreadyDismissed = new(
            "Student.AlreadyDismissed", "Student is already dismissed.");

        public static readonly Error CannotDeleteStudentWithRegistrations = new(
            "Student.CannotDeleteWithRegistrations",
            "Cannot delete a student with active or completed course registrations.");

        public static readonly Error CannotChangeProgramForDismissedStudent = new(
            "Student.CannotChangeProgramForDismissed",
            "Cannot change program for a dismissed student.");

        public static Error DuplicateNationalId = new Error(
            "Student.DuplicateNationalId", "A student with this National ID already exists.");
    }
}
