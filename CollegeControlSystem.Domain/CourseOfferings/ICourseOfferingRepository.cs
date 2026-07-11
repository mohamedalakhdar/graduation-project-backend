using CollegeControlSystem.Domain.Shared;

namespace CollegeControlSystem.Domain.CourseOfferings
{
    public interface ICourseOfferingRepository
    {
        Task<CourseOffering?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Crucial for the "Available Courses" page
        Task<List<CourseOffering>> GetBySemesterAsync(Semester semester, CancellationToken cancellationToken = default);

        // Crucial for checking prerequisites (Did student pass Course X?)
        // Note: This logic might live in Student/Registration repo, but getting Offering by Course is useful.
        Task<List<CourseOffering>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        public Task<bool> ExistsAsync(
        Guid courseId,
        Guid instructorId,
        Semester semester,
        CancellationToken cancellationToken);
        void Add(CourseOffering offering);
        void Update(CourseOffering offering);
        void Delete(CourseOffering offering);
        Task<IEnumerable<CourseOffering>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken);
        // Delete is usually rarely used; usually we 'Archive' or 'Cancel' via status
        Task<List<CourseOffering>> GetAvailableOfferingsAsync(
            Semester? semester,
            Guid? courseId,
            Guid? instructorId,
            CancellationToken cancellationToken = default);

        Task<(List<CourseOffering> Items, int TotalCount)> GetFilteredAsync(
            Semester? semester,
            Guid? courseId,
            Guid? instructorId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
