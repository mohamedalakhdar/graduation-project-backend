namespace CollegeControlSystem.Application.Registrations.GetAvailableCourses
{
    public sealed record AvailableCourseResponse(
            Guid OfferingId,
            string CourseCode,
            string CourseTitle,
            int Credits,
            string InstructorName,
            int AvailableSeats,
            bool IsFull
        );
}
