using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Application.RegistrationPeriods.GetAllRegistrationPeriods
{
    internal sealed class GetAllRegistrationPeriodsQueryHandler
        : IQueryHandler<GetAllRegistrationPeriodsQuery, List<RegistrationPeriodResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllRegistrationPeriodsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<RegistrationPeriodResponse>>> Handle(
            GetAllRegistrationPeriodsQuery request,
            CancellationToken cancellationToken)
        {
            var periods = await _unitOfWork.RegistrationPeriodRepository
                .GetAllAsync(cancellationToken);

            var response = periods.Select(p => new RegistrationPeriodResponse(
                p.Id,
                p.Name,
                p.StartDateUtc,
                p.EndDateUtc,
                p.IsActive,
                p.Semester.Term,
                p.Semester.Year,
                p.CreatedAtUtc)).ToList();

            return Result<List<RegistrationPeriodResponse>>.Success(response);
        }
    }
}
