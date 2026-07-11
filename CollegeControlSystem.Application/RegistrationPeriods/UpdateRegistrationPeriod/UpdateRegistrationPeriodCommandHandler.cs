using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.RegistrationPeriods;
using CollegeControlSystem.Domain.Shared;

namespace CollegeControlSystem.Application.RegistrationPeriods.UpdateRegistrationPeriod
{
    internal sealed class UpdateRegistrationPeriodCommandHandler
        : ICommandHandler<UpdateRegistrationPeriodCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRegistrationPeriodCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            UpdateRegistrationPeriodCommand request,
            CancellationToken cancellationToken)
        {
            var period = await _unitOfWork.RegistrationPeriodRepository
                .GetByIdAsync(request.Id, cancellationToken);

            if (period is null)
                return Result.Failure(RegistrationPeriodErrors.NotFound);

            Semester? semester = null;
            if (request.Term is not null || request.Year is not null)
            {
                var term = request.Term ?? period.Semester.Term;
                var year = request.Year ?? period.Semester.Year;
                var semesterResult = Semester.Create(term, year);
                if (semesterResult.IsFailure)
                    return Result.Failure(semesterResult.Error);
                semester = semesterResult.Value;
            }

            var result = period.Update(
                request.Name,
                request.StartDateUtc,
                request.EndDateUtc,
                request.IsActive,
                semester);

            if (result.IsFailure)
                return result;

            _unitOfWork.RegistrationPeriodRepository.Update(period);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
