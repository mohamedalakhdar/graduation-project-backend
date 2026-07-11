using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class FacultyRepository : IFacultyRepository
    {
        private readonly AppDbContext _context;

        public FacultyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Faculty>()
                .Include(f => f.Department) // Include Dept details
                .Include(f => f.AppUser)    // Include User details (Email)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task<List<Faculty>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<Faculty>()
                .Include(f => f.Department)
                .OrderBy(f => f.FacultyName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Faculty>> GetByStatusAsync(FacultyStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Faculty>()
                .Include(f => f.Department)
                .Where(f => f.Status == status)
                .OrderBy(f => f.FacultyName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Faculty>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Faculty>()
                .Where(f => f.DepartmentId == departmentId)
                .Include(f => f.Department)
                .OrderBy(f => f.FacultyName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Faculty>> GetAdvisorsAsync(CancellationToken cancellationToken = default)
        {
            // NOTE: In many systems, "Advisor" is a Role. 
            // If *all* faculty can be advisors, this returns all.
            // If only specific faculty are advisors, you would need to filter here.
            // For now, assuming all Faculty members are eligible to be Advisors.

            return await _context.Set<Faculty>()
                .Include(f => f.Department)
                .OrderBy(f => f.FacultyName)
                .ToListAsync(cancellationToken);
        }

        public async Task<(List<Faculty> Items, int TotalCount)> GetFilteredAsync(
            string? search,
            Guid? departmentId,
            FacultyStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Faculty>()
                .Include(f => f.Department)
                .Include(f => f.AppUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(f => f.FacultyName.Contains(search));

            if (departmentId.HasValue)
                query = query.Where(f => f.DepartmentId == departmentId.Value);

            if (status.HasValue)
                query = query.Where(f => f.Status == status.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(f => f.FacultyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<(List<Faculty> Items, int TotalCount)> GetAdvisorsFilteredAsync(
            string? search,
            Guid? departmentId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var advisorRoleId = await _context.Set<AppRole>()
                .Where(r => r.Name == Roles.AdvisorRole)
                .Select(r => r.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (advisorRoleId is null)
                return (new List<Faculty>(), 0);

            var query = _context.Set<Faculty>()
                .Include(f => f.Department)
                .Include(f => f.AppUser)
                .Where(f => _context.Set<IdentityUserRole<string>>()
                    .Any(ur => ur.RoleId == advisorRoleId && ur.UserId == f.AppUserId))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(f => f.FacultyName.Contains(search));

            if (departmentId.HasValue)
                query = query.Where(f => f.DepartmentId == departmentId.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(f => f.FacultyName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task AddAsync(Faculty faculty)
        {
            await _context.Set<Faculty>().AddAsync(faculty);
        }

        public void Update(Faculty faculty)
        {
            _context.Set<Faculty>().Update(faculty);
        }
    }
}
