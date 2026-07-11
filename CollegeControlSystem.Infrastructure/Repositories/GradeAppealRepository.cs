using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CollegeControlSystem.Infrastructure.Repositories;

internal sealed class GradeAppealRepository : IGradeAppealRepository
{
    private readonly AppDbContext _context;

    public GradeAppealRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GradeAppeal?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Set<GradeAppeal>()
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<GradeAppeal?> GetByGradeIdAsync(Guid gradeId, CancellationToken ct = default)
    {
        return await _context.Set<GradeAppeal>()
            .FirstOrDefaultAsync(a => a.GradeId == gradeId, ct);
    }

    public async Task AddAsync(GradeAppeal gradeAppeal, CancellationToken ct = default)
    {
        await _context.Set<GradeAppeal>().AddAsync(gradeAppeal, ct);
    }

    public Task UpdateAsync(GradeAppeal gradeAppeal, CancellationToken ct = default)
    {
        _context.Set<GradeAppeal>().Update(gradeAppeal);
        return Task.CompletedTask;
    }
}
