using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Shared;

namespace CollegeControlSystem.Application.ControlEngine.RunEngine
{
    internal static class GpaCalculatorService
    {
        public static GpaResult Calculate(List<Registration> transcript, Semester currentSemester)
        {
            // 1. Filter out incomplete or non-GPA registrations (W, I, P)
            var validRegistrations = transcript
                .Where(r => r.Status == RegistrationStatus.Completed &&
                            r.Grade != null &&
                            r.Grade.LetterGrade != null)
                .ToList();

            // 2. Calculate SGPA (Current Semester Only)
            var semesterRegistrations = validRegistrations
                .Where(r => r.CourseOffering.Semester.Year == currentSemester.Year &&
                            r.CourseOffering.Semester.Term == currentSemester.Term &&
                            GradeScale.FromLetter(r.Grade.LetterGrade).Value.IsIncludedInGpa())
                .ToList();

            decimal sgpa = 0.00m;
            int semesterCredits = semesterRegistrations.Sum(r => r.CourseOffering.Course.Credits);

            if (semesterCredits > 0)
            {
                decimal totalSemesterPoints = semesterRegistrations
                    .Sum(r => r.Grade.GradePoints * r.CourseOffering.Course.Credits);
                sgpa = totalSemesterPoints / semesterCredits;
            }

            // 3. Calculate CGPA & Apply Retake Policy (Article 28)
            // Group by CourseId. If a student took a course twice, we only count the highest grade 
            // (Note: The Domain `Grade.CalculateAndSet` already capped retakes at 3.3/B+ when the professor submitted the grade)
            var uniqueCoursesForCgpa = validRegistrations
                .Where(r => GradeScale.FromLetter(r.Grade.LetterGrade).Value.IsIncludedInGpa())
                .GroupBy(r => r.CourseOffering.CourseId)
                .Select(group => group.OrderByDescending(r => r.Grade.GradePoints).First()) // Take highest grade attempt
                .ToList();

            decimal cgpa = 0.00m;
            int totalAttemptedCredits = uniqueCoursesForCgpa.Sum(r => r.CourseOffering.Course.Credits);

            if (totalAttemptedCredits > 0)
            {
                decimal totalCumulativePoints = uniqueCoursesForCgpa
                    .Sum(r => r.Grade.GradePoints * r.CourseOffering.Course.Credits);
                cgpa = totalCumulativePoints / totalAttemptedCredits;
            }

            // 4. Calculate Total Earned (Passed) Credits for Academic Level (Article 20)
            // This includes 'P' (Pass) grades which don't count in CGPA but DO count for graduation credits.
            int earnedCredits = transcript
                .Where(r => r.Status == RegistrationStatus.Completed && r.Grade != null && r.Grade.IsPassing)
                .GroupBy(r => r.CourseOffering.CourseId)
                .Select(g => g.First()) // Count the course only once
                .Sum(r => r.CourseOffering.Course.Credits);

            // Round GPAs to 2 decimal places
            return new GpaResult(
                Math.Round(sgpa, 2),
                Math.Round(cgpa, 2),
                earnedCredits,
                semesterCredits > 0 // Did they actually study this semester?
            );
        }
    }

    internal record GpaResult(decimal SGPA, decimal CGPA, int EarnedCredits, bool IsActiveThisSemester);
}