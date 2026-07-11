namespace CollegeControlSystem.Domain.Registrations
{
    public interface IRegistrationRepository
    {
        Task<Registration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Used to check: "Did this student already register for this Offering?"
        Task<Registration?> GetByStudentAndOfferingAsync(
            Guid studentId,
            Guid courseOfferingId,
            CancellationToken cancellationToken = default);

        // Used to check prerequisites: "Has student passed Course X before?"
        // Returns list because they might have failed it twice then passed it.
        Task<List<Registration>> GetByStudentAndCourseIdAsync(
            Guid studentId,
            Guid courseId,
            CancellationToken cancellationToken = default);

        // Used for Advisor Dashboard: "Show me pending requests"
        Task<List<Registration>> GetPendingByAdvisorIdAsync(
            Guid advisorId,
            CancellationToken cancellationToken = default);

        void Add(Registration registration);
        void Update(Registration registration);
        Task<List<Registration>> GetByStudentAndSemesterAsync(Guid studentId, string term, int value, CancellationToken cancellationToken = default);
        Task<List<Registration>> GetActiveByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);

        //Fetch all students registered for a specific offering
        Task<List<Registration>> GetByOfferingIdAsync(
            Guid courseOfferingId,
            CancellationToken cancellationToken = default);

        Task<List<Registration>> GetAllByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);

        Task<(List<Registration> Items, int TotalCount)> GetFilteredAsync(
            RegistrationStatus? status,
            Guid? studentId,
            Guid? courseOfferingId,
            string? semester,
            int? year,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}

