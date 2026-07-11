using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.RegistrationPeriods;

namespace CollegeControlSystem.Application.RegistrationPeriods.GetCurrentRegistrationPeriod
{
    internal sealed class GetCurrentRegistrationPeriodQueryHandler
        : IQueryHandler<GetCurrentRegistrationPeriodQuery, RegistrationPeriodResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCurrentRegistrationPeriodQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RegistrationPeriodResponse>> Handle(
            GetCurrentRegistrationPeriodQuery request,
            CancellationToken cancellationToken)
        {
            var period = await _unitOfWork.RegistrationPeriodRepository
                .GetCurrentActivePeriodAsync(cancellationToken);

            if (period is null)
                return Result<RegistrationPeriodResponse>.Failure(RegistrationPeriodErrors.NotFound);

            return Result<RegistrationPeriodResponse>.Success(Map(period));
        }

        private static RegistrationPeriodResponse Map(RegistrationPeriod period) =>
            new(
                period.Id,
                period.Name,
                period.StartDateUtc,
                period.EndDateUtc,
                period.IsActive,
                period.Semester.Term,
                period.Semester.Year,
                period.CreatedAtUtc);
    }
}
