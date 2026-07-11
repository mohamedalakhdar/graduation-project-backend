using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.CreateDepartment
{
    internal sealed class CreateDepartmentCommandHandler : ICommandHandler<CreateDepartmentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateDepartmentCommandHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var result = Department.Create(request.Name, request.Description);

            if (result.IsFailure)
            {
                return Result<Guid>.Failure(result.Error);
            }

            var department = result.Value;

            _unitOfWork.DepartmentRepository.Add(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(department.Id);
        }
    }
}
