using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.ChangeFacultyStatus
{
    internal sealed class ChangeFacultyStatusCommandHandler : ICommandHandler<ChangeFacultyStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeFacultyStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ChangeFacultyStatusCommand request, CancellationToken cancellationToken)
        {
            var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.FacultyId, cancellationToken);

            if (faculty is null)
                return Result.Failure(FacultyErrors.NotFound);

            if (faculty.Status == request.NewStatus)
                return Result.Failure(FacultyErrors.SameStatus);

            faculty.ChangeStatus(request.NewStatus);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
