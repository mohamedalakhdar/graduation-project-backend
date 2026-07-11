using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class GradeRepository : IGradeRepository
    {
        private readonly AppDbContext _context;

        public GradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Grade?> GetByRegistrationIdAsync(Guid registrationId)
        {
            return await _context.Set<Grade>()
                .FirstOrDefaultAsync(g => g.RegistrationId == registrationId);
        }

        public void Add(Grade grade)
        {
            _context.Set<Grade>().Add(grade);
        }
    }
}
