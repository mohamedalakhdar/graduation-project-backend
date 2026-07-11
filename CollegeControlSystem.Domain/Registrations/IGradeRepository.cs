namespace CollegeControlSystem.Domain.Registrations
{
    public interface IGradeRepository
    {
        Task<Grade?> GetByRegistrationIdAsync(Guid registrationId);
        void Add(Grade grade);
    }
}
