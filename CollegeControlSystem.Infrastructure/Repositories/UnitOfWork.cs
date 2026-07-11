using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Courses;
using CollegeControlSystem.Domain.Departments;
using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Domain.RegistrationPeriods;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;
using CollegeControlSystem.Infrastructure.Database;

namespace CollegeControlSystem.Infrastructure.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            StudentRepository = new StudentRepository(_appDbContext);
            FacultyRepository = new FacultyRepository(_appDbContext);
            DepartmentRepository = new DepartmentRepository(_appDbContext);
            CourseOfferingRepository = new CourseOfferingRepository(_appDbContext);
            CourseRepository = new CourseRepository(_appDbContext);
            GradeRepository = new GradeRepository(_appDbContext);
            RegistrationRepository = new RegistrationRepository(_appDbContext);
            GradeAppealRepository = new GradeAppealRepository(_appDbContext);
            RegistrationPeriodRepository = new RegistrationPeriodRepository(_appDbContext);
        }

        public IStudentRepository StudentRepository { get; private set; }

        public IFacultyRepository FacultyRepository { get; private set; }

        public IDepartmentRepository DepartmentRepository { get; private set; }

        public ICourseOfferingRepository CourseOfferingRepository { get; private set; }

        public ICourseRepository CourseRepository { get; private set; }

        public IGradeRepository GradeRepository { get; private set; }

        public IRegistrationRepository RegistrationRepository { get; private set; }

        public IGradeAppealRepository GradeAppealRepository { get; private set; }

        public IRegistrationPeriodRepository RegistrationPeriodRepository { get; private set; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
