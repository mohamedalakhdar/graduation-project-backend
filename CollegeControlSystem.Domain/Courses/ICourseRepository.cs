
namespace CollegeControlSystem.Domain.Courses
{
    public interface ICourseRepository
    {
        // Get course with parsed Code Value Object
        Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Check uniqueness of code "ABC 123"
        Task<bool> IsCodeUniqueAsync(string normalizedCode, CancellationToken cancellationToken = default);

        void Add(Course course);
        Task AddAsync(Course course, CancellationToken cancellationToken = default);
        void Update(Course course);
        void Delete(Course course);
        Task<List<Course>> GetByDepartmentAsync(Guid? departmentId, CancellationToken cancellationToken);
        // Note: Ideally, your Repository's GetByIdWithPrerequisitesAsync should Include(c => c.Prerequisites).ThenInclude(p => p.PrerequisiteCourse)
        Task<Course?> GetByIdWithPrerequisitesAsync(Guid id, CancellationToken ct = default);

        Task<(List<Course> Items, int TotalCount)> GetFilteredAsync(
            string? search,
            Guid? departmentId,
            int? minCredits,
            int? maxCredits,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
