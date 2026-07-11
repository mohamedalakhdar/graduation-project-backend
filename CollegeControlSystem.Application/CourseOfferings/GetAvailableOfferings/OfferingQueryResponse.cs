namespace CollegeControlSystem.Application.CourseOfferings.GetAvailableOfferings
{
    public sealed record OfferingQueryResponse(
            Guid OfferingId,
            string CourseCode,
            string CourseTitle,
            string InstructorName,
            int Capacity,
            int Enrolled,
            bool IsFull
        );
}
