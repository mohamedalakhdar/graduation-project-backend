namespace CollegeControlSystem.Domain.Students
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Used during registration to check duplicates
        Task<Student?> GetByAcademicNumberAsync(string academicNumber, CancellationToken cancellationToken = default);

        // Used by Advisor Dashboard
        Task<List<Student>> GetByAdvisorIdAsync(Guid advisorId, CancellationToken cancellationToken = default);

        // Used for Control Engine (Batch Processing)
        Task<List<Student>> GetAllActiveAsync(CancellationToken cancellationToken = default);

        // Used to fetch all students (admin list view)
        Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<List<Student>> GetByStatusAsync(AcademicStatus status, CancellationToken cancellationToken = default);

        Task AddAsync(Student student);
        void Update(Student student);
        void Delete(Student student);
        Task<Student?> GetByIdWithProgramAsync(Guid studentId, CancellationToken cancellationToken);

        Task<Student?> GetByAppUserIdAsync(string appUserId, CancellationToken cancellationToken);
        //Fetch Student with deep includes (Registrations -> Offerings -> Course)
        Task<Student> GetTranscriptDataAsync(Guid studentId, CancellationToken cancellationToken);
        Task<List<Student>> GetStudentsWithWarningsAsync(CancellationToken cancellationToken = default);
        Task<List<Student>> GetStudentsForGraduationAuditAsync(CancellationToken cancellationToken = default);
        // NEW: Fetches active students with their full transcripts for the Control Engine
        Task<List<Student>> GetActiveStudentsWithTranscriptsAsync(CancellationToken cancellationToken = default);

        Task<(List<Student> Items, int TotalCount)> GetFilteredAsync(
            string? search,
            Guid? programId,
            Guid? advisorId,
            AcademicStatus? status,
            decimal? minCGPA,
            decimal? maxCGPA,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<bool> IsNationalIdUnique(string nationalId, CancellationToken cancellationToken);
    }
}
