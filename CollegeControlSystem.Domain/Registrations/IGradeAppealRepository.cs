namespace CollegeControlSystem.Domain.Registrations;

public interface IGradeAppealRepository
{
    Task<GradeAppeal?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<GradeAppeal?> GetByGradeIdAsync(Guid gradeId, CancellationToken ct = default);
    Task AddAsync(GradeAppeal gradeAppeal, CancellationToken ct = default);
    Task UpdateAsync(GradeAppeal gradeAppeal, CancellationToken ct = default);
}
