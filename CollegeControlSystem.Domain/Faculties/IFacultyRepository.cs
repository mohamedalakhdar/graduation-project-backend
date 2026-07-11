namespace CollegeControlSystem.Domain.Faculties
{
    public interface IFacultyRepository
    {
        Task<Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);


        // Useful for "Show me all professors in Computer Engineering"
        Task<List<Faculty>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default);

        // Useful for assigning an advisor to a student
        Task<List<Faculty>> GetAdvisorsAsync(CancellationToken cancellationToken = default);

        Task AddAsync(Faculty faculty);
        void Update(Faculty faculty);
        Task<List<Faculty>> GetAllAsync(CancellationToken cancellationToken);

        Task<List<Faculty>> GetByStatusAsync(FacultyStatus status, CancellationToken cancellationToken = default);

        Task<(List<Faculty> Items, int TotalCount)> GetFilteredAsync(
            string? search,
            Guid? departmentId,
            FacultyStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<(List<Faculty> Items, int TotalCount)> GetAdvisorsFilteredAsync(
            string? search,
            Guid? departmentId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
