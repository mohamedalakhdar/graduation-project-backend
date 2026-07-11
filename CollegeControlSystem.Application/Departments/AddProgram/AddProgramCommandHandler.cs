using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.AddProgram
{
    internal sealed class AddProgramCommandHandler : ICommandHandler<AddProgramCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddProgramCommandHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddProgramCommand request, CancellationToken cancellationToken)
        {
            // 1. Load Aggregate Root
            var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);

            if (department is null)
            {
                return Result<Guid>.Failure(DepartmentErrors.NotFound);
            }

            // 2. Check for duplicates within this department
            if (department.Programs.Any(p => p.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Result<Guid>.Failure(DepartmentErrors.DuplicateProgram);
            }

            // 3. Create Program via factory (domain validation)
            var programResult = Program.Create(request.Name, request.RequiredCredits, request.DepartmentId);

            if (programResult.IsFailure)
            {
                return Result<Guid>.Failure(programResult.Error);
            }

            // 4. Persist via repository (ensures EF Core tracks as Added)
            await _unitOfWork.DepartmentRepository.AddProgramAsync(programResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Return the new Program ID
            return Result<Guid>.Success(programResult.Value.Id);
        }
    }
}
