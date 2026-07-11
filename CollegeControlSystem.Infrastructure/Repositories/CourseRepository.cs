using CollegeControlSystem.Domain.Courses;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Course>()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Course?> GetByIdWithPrerequisitesAsync(Guid id, CancellationToken ct = default)
        {
            // This is crucial for the "Get Course Details" query
            return await _context.Set<Course>()
                .Include(c => c.Prerequisites)
                    .ThenInclude(cp => cp.PrerequisiteCourse) // Load the actual Prereq Course Entity
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<bool> IsCodeUniqueAsync(string normalizedCode, CancellationToken cancellationToken = default)
        {
            // Because of the HasConversion, we need to compare apples to apples.
            // Option 1: Client evaluation (Slow)
            // Option 2: Construct the Value Object to compare against.

            //var codeResult = CourseCode.Create(normalizedCode);
            //if (codeResult.IsFailure) return false; // Invalid code format implies it doesn't exist in DB anyway

            return !await _context.Set<Course>()
                .AnyAsync(c => c.Code.Equals(normalizedCode), cancellationToken);
        }

        public async Task<List<Course>> GetByDepartmentAsync(Guid? departmentId, CancellationToken cancellationToken)
        {
            var query = _context.Set<Course>().AsQueryable();

            if (departmentId.HasValue)
            {
                query = query.Where(c => c.DepartmentId == departmentId.Value);
            }

            return await query
                .OrderBy(c => c.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task<(List<Course> Items, int TotalCount)> GetFilteredAsync(
            string? search,
            Guid? departmentId,
            int? minCredits,
            int? maxCredits,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Course>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Title.Contains(search));

            if (departmentId.HasValue)
                query = query.Where(c => c.DepartmentId == departmentId.Value);

            if (minCredits.HasValue)
                query = query.Where(c => c.Credits >= minCredits.Value);

            if (maxCredits.HasValue)
                query = query.Where(c => c.Credits <= maxCredits.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(c => c.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public void Add(Course course)
        {
            _context.Set<Course>().Add(course);
        }

        public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
        {
            await _context.Set<Course>().AddAsync(course, cancellationToken);
        }

        public void Update(Course course)
        {
            _context.Set<Course>().Update(course);
        }

        public void Delete(Course course)
        {
            _context.Set<Course>().Remove(course);
        }
    }
}
