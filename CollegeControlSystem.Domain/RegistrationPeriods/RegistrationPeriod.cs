using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Shared;

namespace CollegeControlSystem.Domain.RegistrationPeriods
{
    public sealed class RegistrationPeriod : Entity
    {
        private RegistrationPeriod() { }

        private RegistrationPeriod(
            Guid id,
            string name,
            DateTime startDateUtc,
            DateTime endDateUtc,
            bool isActive,
            Semester semester) : base(id)
        {
            Name = name;
            StartDateUtc = startDateUtc;
            EndDateUtc = endDateUtc;
            IsActive = isActive;
            Semester = semester;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public string Name { get; private set; }
        public DateTime StartDateUtc { get; private set; }
        public DateTime EndDateUtc { get; private set; }
        public bool IsActive { get; private set; }
        public Semester Semester { get; private set; }
        public DateTime CreatedAtUtc { get; private init; }

        public bool IsOpenForRegistration() =>
            IsActive && DateTime.UtcNow >= StartDateUtc && DateTime.UtcNow <= EndDateUtc;

        public static Result<RegistrationPeriod> Create(
            string name,
            DateTime startDateUtc,
            DateTime endDateUtc,
            bool isActive,
            Semester semester)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<RegistrationPeriod>.Failure(RegistrationPeriodErrors.NameRequired);

            if (startDateUtc >= endDateUtc)
                return Result<RegistrationPeriod>.Failure(RegistrationPeriodErrors.InvalidDateRange);

            var period = new RegistrationPeriod(
                Guid.NewGuid(), name, startDateUtc, endDateUtc, isActive, semester);

            return Result<RegistrationPeriod>.Success(period);
        }

        public Result Update(
            string? name,
            DateTime? startDateUtc,
            DateTime? endDateUtc,
            bool? isActive,
            Semester? semester)
        {
            if (startDateUtc.HasValue && endDateUtc.HasValue && startDateUtc.Value >= endDateUtc.Value)
                return Result.Failure(RegistrationPeriodErrors.InvalidDateRange);

            if (startDateUtc.HasValue && !endDateUtc.HasValue && startDateUtc.Value >= EndDateUtc)
                return Result.Failure(RegistrationPeriodErrors.InvalidDateRange);

            if (!startDateUtc.HasValue && endDateUtc.HasValue && StartDateUtc >= endDateUtc.Value)
                return Result.Failure(RegistrationPeriodErrors.InvalidDateRange);

            if (name is not null) Name = name;
            if (startDateUtc.HasValue) StartDateUtc = startDateUtc.Value;
            if (endDateUtc.HasValue) EndDateUtc = endDateUtc.Value;
            if (isActive.HasValue) IsActive = isActive.Value;
            if (semester is not null) Semester = semester;

            return Result.Success();
        }

        public Result Activate()
        {
            if (IsActive)
                return Result.Failure(RegistrationPeriodErrors.AlreadyActive);

            IsActive = true;
            return Result.Success();
        }

        public Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure(RegistrationPeriodErrors.AlreadyInactive);

            IsActive = false;
            return Result.Success();
        }
    }
}
