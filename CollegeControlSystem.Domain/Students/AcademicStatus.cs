namespace CollegeControlSystem.Domain.Students
{
    public enum AcademicStatus
    {
        GoodStanding = 1,

        // "SGPA < 2.00 in any main semester" [cite: 197]
        AcademicWarning = 2,

        // "Four consecutive متتاليه academic warnings" 
        Dismissed = 3,
        Graduated=4,
    }
}
