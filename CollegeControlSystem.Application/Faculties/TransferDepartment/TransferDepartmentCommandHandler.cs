using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.TransferDepartment
{
    internal sealed class TransferDepartmentCommandHandler : ICommandHandler<TransferDepartmentCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransferDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(TransferDepartmentCommand request, CancellationToken cancellationToken)
        {
            var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.FacultyId, cancellationToken);

            if (faculty is null)
            {
                return Result.Failure(FacultyErrors.NotFound);
            }

            var result = faculty.TransferDepartment(request.NewDepartmentId);

            if (result.IsFailure) return result;

            // TODO: Does i need update function here?
            //_unitOfWork.FacultyRepository.Update(faculty);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
