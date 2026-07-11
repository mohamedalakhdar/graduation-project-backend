using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.RegistrationPeriods;

namespace CollegeControlSystem.Application.RegistrationPeriods.DeleteRegistrationPeriod
{
    internal sealed class DeleteRegistrationPeriodCommandHandler
        : ICommandHandler<DeleteRegistrationPeriodCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRegistrationPeriodCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            DeleteRegistrationPeriodCommand request,
            CancellationToken cancellationToken)
        {
            var period = await _unitOfWork.RegistrationPeriodRepository
                .GetByIdAsync(request.Id, cancellationToken);

            if (period is null)
                return Result.Failure(RegistrationPeriodErrors.NotFound);

            _unitOfWork.RegistrationPeriodRepository.Delete(period);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
