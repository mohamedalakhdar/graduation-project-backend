using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Shared;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class CourseOfferingRepository : ICourseOfferingRepository
    {
        private readonly AppDbContext _context;

        public CourseOfferingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CourseOffering?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // We generally need Course and Instructor details when fetching a single offering
            return await _context.Set<CourseOffering>()
                .Include(co => co.Course)
                .Include(co => co.Instructor)
                .FirstOrDefaultAsync(co => co.Id == id, cancellationToken);
        }

        public async Task<List<CourseOffering>> GetBySemesterAsync(Semester semester, CancellationToken cancellationToken = default)
        {
            // Used for "Available Courses" page
            return await _context.Set<CourseOffering>()
                .Include(co => co.Course)           // Include Title, Credits
                .Include(co => co.Instructor)       // Include Instructor Name
                                                    // Querying Value Object properties
                .Where(co => !co.IsCancelled)
                .Where(co => co.Semester.Year == semester.Year && co.Semester.Term == semester.Term)
                //.OrderBy(co => co.Course.Code.Value)
                .OrderBy(c => EF.Property<string>(c.Course, "Code"))
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CourseOffering>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<CourseOffering>()
                .Where(co => co.CourseId == courseId)
                .OrderByDescending(co => co.Semester.Year)
                .ThenByDescending(co => co.Semester.Term)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CourseOffering>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken)
        {
            return await _context.Set<CourseOffering>()
                .Include(co => co.Course)
                .Where(co => co.InstructorId == instructorId)
                .OrderByDescending(co => co.Semester.Year)
                .ThenByDescending(co => co.Semester.Term)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(
        Guid courseId,
        Guid instructorId,
        Semester semester,
        CancellationToken cancellationToken)
        {
            return await _context.Set<CourseOffering>().AnyAsync(x =>
                x.CourseId == courseId &&
                x.InstructorId == instructorId &&
                x.Semester.Term == semester.Term &&
                x.Semester.Year == semester.Year,
                cancellationToken);
        }
        public void Add(CourseOffering offering)
        {
            _context.Set<CourseOffering>().Add(offering);
        }

        public void Update(CourseOffering offering)
        {
            _context.Set<CourseOffering>().Update(offering);
        }

        public void Delete(CourseOffering offering)
        {
            _context.Set<CourseOffering>().Remove(offering);
        }

        public async Task<List<CourseOffering>> GetAvailableOfferingsAsync(
            Semester? semester,
            Guid? courseId,
            Guid? instructorId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<CourseOffering>()
                .Include(co => co.Course)
                .Include(co => co.Instructor)
                .AsQueryable();

            query = query.Where(co => !co.IsCancelled);

            if (semester is not null)
            {
                query = query.Where(co => co.Semester.Year == semester.Year && co.Semester.Term == semester.Term);
            }

            if (courseId.HasValue)
            {
                query = query.Where(co => co.CourseId == courseId.Value);
            }

            if (instructorId.HasValue)
            {
                query = query.Where(co => co.InstructorId == instructorId.Value);
            }

            return await query
                .OrderByDescending(co => co.Semester.Year)
                .ThenByDescending(co => co.Semester.Term)
                .ThenBy(co => co.Course.Title)
                .ToListAsync(cancellationToken);
        }

        public async Task<(List<CourseOffering> Items, int TotalCount)> GetFilteredAsync(
            Semester? semester,
            Guid? courseId,
            Guid? instructorId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<CourseOffering>()
                .Include(co => co.Course)
                .Include(co => co.Instructor)
                .AsQueryable();

            query = query.Where(co => !co.IsCancelled);

            if (semester is not null)
            {
                query = query.Where(co => co.Semester.Year == semester.Year && co.Semester.Term == semester.Term);
            }

            if (courseId.HasValue)
            {
                query = query.Where(co => co.CourseId == courseId.Value);
            }

            if (instructorId.HasValue)
            {
                query = query.Where(co => co.InstructorId == instructorId.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(co => co.Semester.Year)
                .ThenByDescending(co => co.Semester.Term)
                .ThenBy(co => co.Course.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
