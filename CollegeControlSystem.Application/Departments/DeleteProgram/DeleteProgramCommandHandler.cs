using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.DeleteProgram
{
    internal sealed class DeleteProgramCommandHandler : ICommandHandler<DeleteProgramCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProgramCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteProgramCommand request, CancellationToken cancellationToken)
        {
            //var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);

            //if (department is null)
            //{
            //    return Result.Failure(DepartmentErrors.NotFound);
            //}

            //var program = department.GetProgram(request.ProgramId);
            var program = await _unitOfWork.DepartmentRepository.GetProgramByIdAsync(request.ProgramId, cancellationToken);
            if (program is null)
            {
                return Result.Failure(DepartmentErrors.ProgramNotFound);
            }

            var hasStudents = await _unitOfWork.DepartmentRepository.HasStudentsAsync(request.ProgramId, cancellationToken);
            if (hasStudents)
            {
                return Result.Failure(DepartmentErrors.ProgramHasStudents);
            }

             _unitOfWork.DepartmentRepository.RemoveProgram(program);

            //var removeResult = department.RemoveProgram(request.ProgramId);
            //if (removeResult.IsFailure)
            //{
            //    return removeResult;
            //}

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
