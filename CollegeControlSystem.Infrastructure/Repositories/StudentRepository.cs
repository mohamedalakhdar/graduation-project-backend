using CollegeControlSystem.Domain.Students;
using CollegeControlSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;


namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal sealed class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Student?> GetByAcademicNumberAsync(string academicNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .FirstOrDefaultAsync(s => s.AcademicNumber == academicNumber, cancellationToken);
        }

        public async Task<List<Student>> GetByAdvisorIdAsync(Guid advisorId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .Include(s => s.Program) // Often needed for the Advisor Dashboard
                .Where(s => s.AdvisorId == advisorId)
                .OrderBy(s => s.StudentName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .Include(s => s.Program)
                .OrderBy(s => s.AcademicNumber)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Student>> GetByStatusAsync(AcademicStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .Include(s => s.Program)
                .Where(s => s.AcademicStatus == status)
                .OrderBy(s => s.AcademicNumber)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Student>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        {
            // "Active" typically means not Dismissed (and optionally not Graduated, depending on requirements).
           // Based on[cite: 197], Dismissed is a terminal status.
            return await _context.Set<Student>()
                .Where(s => s.AcademicStatus == AcademicStatus.GoodStanding || s.AcademicStatus==AcademicStatus.AcademicWarning)
                .ToListAsync(cancellationToken);
        }

        public async Task<(List<Student> Items, int TotalCount)> GetFilteredAsync(
            string? search,
            Guid? programId,
            Guid? advisorId,
            AcademicStatus? status,
            decimal? minCGPA,
            decimal? maxCGPA,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Student>()
                .Include(s => s.Program)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(s => s.StudentName.Contains(search));

            if (programId.HasValue)
                query = query.Where(s => s.ProgramId == programId.Value);

            if (advisorId.HasValue)
                query = query.Where(s => s.AdvisorId == advisorId.Value);

            if (status.HasValue)
                query = query.Where(s => s.AcademicStatus == status.Value);

            if (minCGPA.HasValue)
                query = query.Where(s => s.CGPA >= minCGPA.Value);

            if (maxCGPA.HasValue)
                query = query.Where(s => s.CGPA <= maxCGPA.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(s => s.AcademicNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task AddAsync(Student student)
        {
            await _context.Set<Student>().AddAsync(student);
        }

        public void Update(Student student)
        {
            _context.Set<Student>().Update(student);
        }

        public void Delete(Student student)
        {
            _context.Set<Student>().Remove(student);
        }

        public async Task<Student> GetByIdWithProgramAsync(Guid studentId, CancellationToken cancellationToken)
        {
            return await _context.Set<Student>()
                .Include(s => s.Program)
                .Include(s => s.AppUser) // Helpful to get Email
                .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
        }

        public async Task<Student> GetTranscriptDataAsync(Guid studentId, CancellationToken cancellationToken)
        {
            // Deep include to fetch the hierarchy: Student -> Registrations -> CourseOffering -> Course
            // This is necessary for generating the Transcript Report.
            return await _context.Set<Student>()
                .Include(s => s.Program) // Transcript Header info
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.CourseOffering)
                        .ThenInclude(co => co.Course) // Actual Course Details (Code, Credits)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Grade) // The Grades achieved
                .AsSplitQuery() // Optimization for deep includes to avoid Cartesian explosion
                .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
        }

        public async Task<Student?> GetByAppUserIdAsync(string appUserId, CancellationToken cancellationToken)
        {
            return await _context.Set<Student>()
                .Include(s => s.Program)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.CourseOffering)
                        .ThenInclude(co => co.Course)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Grade)
                .AsSplitQuery()
                .FirstOrDefaultAsync(s => s.AppUserId == appUserId, cancellationToken);
        }

        public async Task<List<Student>> GetStudentsWithWarningsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .Include(s => s.Program) // Include Program to show their major in the report
                .Where(s =>
                    s.AcademicStatus == AcademicStatus.AcademicWarning ||
                    s.AcademicStatus == AcademicStatus.Dismissed)
                .OrderByDescending(s => s.ConsecutiveWarnings) // Most critical cases first
                .ThenBy(s => s.StudentName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Student>> GetStudentsForGraduationAuditAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .Include(s => s.Program)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.CourseOffering)
                        .ThenInclude(o => o.Course)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Grade)
                // Filter 1: Only check Senior 2 students (Level 4) with CGPA >= 2.00
                .Where(s => s.AcademicLevel == AcademicLevel.Senior2 && s.CGPA >= 2.00m)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Student>> GetActiveStudentsWithTranscriptsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Student>()
                .Include(s => s.Program)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.CourseOffering)
                        .ThenInclude(o => o.Course)
                .Include(s => s.Registrations)
                    .ThenInclude(r => r.Grade)
                // Only process students who are currently studying (exclude graduated or already dismissed)
                .Where(s =>
                    s.AcademicStatus == AcademicStatus.GoodStanding ||
                    s.AcademicStatus == AcademicStatus.AcademicWarning)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsNationalIdUnique(string nationalId, CancellationToken cancellationToken)
        {
            return !await _context.Set<Student>()
                .AnyAsync(s => s.NationalId == nationalId, cancellationToken);
        }
    }
}