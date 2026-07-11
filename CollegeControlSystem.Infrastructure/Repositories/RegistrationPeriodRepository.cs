using CollegeControlSystem.Domain.RegistrationPeriods;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class RegistrationPeriodRepository : IRegistrationPeriodRepository
    {
        private readonly AppDbContext _context;

        public RegistrationPeriodRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RegistrationPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Set<RegistrationPeriod>()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task<RegistrationPeriod?> GetCurrentActivePeriodAsync(CancellationToken cancellationToken = default)
            => await _context.Set<RegistrationPeriod>()
                .Where(p => p.IsActive && p.StartDateUtc <= DateTime.UtcNow && p.EndDateUtc >= DateTime.UtcNow)
                .OrderByDescending(p => p.CreatedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<List<RegistrationPeriod>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.Set<RegistrationPeriod>()
                .OrderByDescending(p => p.Semester.Year)
                .ThenByDescending(p => p.Semester.Term)
                .ToListAsync(cancellationToken);

        public async Task AddAsync(RegistrationPeriod period)
            => await _context.Set<RegistrationPeriod>().AddAsync(period);

        public void Update(RegistrationPeriod period)
            => _context.Set<RegistrationPeriod>().Update(period);

        public void Delete(RegistrationPeriod period)
            => _context.Set<RegistrationPeriod>().Remove(period);
    }
}
