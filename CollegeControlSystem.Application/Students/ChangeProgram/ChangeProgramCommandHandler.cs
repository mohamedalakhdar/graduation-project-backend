using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.ChangeProgram
{
    internal sealed class ChangeProgramCommandHandler : ICommandHandler<ChangeProgramCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeProgramCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ChangeProgramCommand request, CancellationToken cancellationToken)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.StudentId, cancellationToken);

            if (student is null)
                return Result.Failure(StudentErrors.StudentNotFound);

            var newProgram = await _unitOfWork.DepartmentRepository.GetProgramByIdAsync(request.NewProgramId, cancellationToken);

            if (newProgram is null)
                return Result.Failure(DepartmentErrors.ProgramNotFound);

            var result = student.ChangeProgram(request.NewProgramId);

            if (result.IsFailure)
                return result;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
