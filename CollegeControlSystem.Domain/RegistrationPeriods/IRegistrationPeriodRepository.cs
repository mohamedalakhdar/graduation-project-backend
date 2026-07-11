namespace CollegeControlSystem.Domain.RegistrationPeriods
{
    public interface IRegistrationPeriodRepository
    {
        Task<RegistrationPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<RegistrationPeriod?> GetCurrentActivePeriodAsync(CancellationToken cancellationToken = default);
        Task<List<RegistrationPeriod>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(RegistrationPeriod period);
        void Update(RegistrationPeriod period);
        void Delete(RegistrationPeriod period);
    }
}
