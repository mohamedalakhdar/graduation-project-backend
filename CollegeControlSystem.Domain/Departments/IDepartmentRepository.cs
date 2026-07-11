
namespace CollegeControlSystem.Domain.Departments
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        //include Programs when fetching departments
        Task<List<Department>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<(List<Department> Items, int TotalCount)> GetFilteredAsync(
            string? search,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        // Useful for validation "Does Department CCE exist?"
        Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        void Add(Department department);
        Task AddProgramAsync(Program program, CancellationToken cancellationToken = default);
        void Update(Department department);
        void Delete(Department department);

        // populate Program with Department data for UI context
        Task<List<Program>> GetProgramsWithDepartmentAsync(CancellationToken cancellationToken = default);

        Task<(List<Program> Items, int TotalCount)> GetProgramsFilteredAsync(
            string? search,
            Guid? departmentId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Program?> GetProgramByIdAsync(Guid programId, CancellationToken cancellationToken = default);

        Task<bool> HasStudentsAsync(Guid programId, CancellationToken cancellationToken = default);

        void RemoveProgram(Program program);
    }
}
