using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.RegistrationPeriods;
using CollegeControlSystem.Domain.Shared;

namespace CollegeControlSystem.Application.RegistrationPeriods.CreateRegistrationPeriod
{
    internal sealed class CreateRegistrationPeriodCommandHandler
        : ICommandHandler<CreateRegistrationPeriodCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRegistrationPeriodCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(
            CreateRegistrationPeriodCommand request,
            CancellationToken cancellationToken)
        {
            var semesterResult = Semester.Create(request.Term, request.Year);
            if (semesterResult.IsFailure)
                return Result<Guid>.Failure(semesterResult.Error);

            var periodResult = RegistrationPeriod.Create(
                request.Name,
                request.StartDateUtc,
                request.EndDateUtc,
                request.IsActive,
                semesterResult.Value);

            if (periodResult.IsFailure)
                return Result<Guid>.Failure(periodResult.Error);

            await _unitOfWork.RegistrationPeriodRepository.AddAsync(periodResult.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(periodResult.Value.Id);
        }
    }
}
