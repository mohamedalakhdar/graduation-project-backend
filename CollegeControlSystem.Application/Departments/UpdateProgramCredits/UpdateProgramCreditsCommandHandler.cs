using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.UpdateProgramCredits
{
    internal sealed class UpdateProgramCreditsCommandHandler : ICommandHandler<UpdateProgramCreditsCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProgramCreditsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateProgramCreditsCommand request, CancellationToken cancellationToken)
        {
            //var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
            //if (department is null)
            //{
            //    return Result.Failure(DepartmentErrors.NotFound);
            //}

            //var result = department.UpdateProgramCredits(request.ProgramId, request.NewRequiredCredits);
            var program = await _unitOfWork.DepartmentRepository.GetProgramByIdAsync(request.ProgramId, cancellationToken);
            if (program is null)
            {
                return Result.Failure(DepartmentErrors.ProgramNotFound);
            }
            var result =program.UpdateCredits(request.NewRequiredCredits);
            if (result.IsFailure)
            {
                return result;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }

}
