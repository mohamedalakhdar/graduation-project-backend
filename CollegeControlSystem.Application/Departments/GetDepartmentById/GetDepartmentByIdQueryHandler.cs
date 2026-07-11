using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Departments.GetDepartments;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.GetDepartmentById
{
    internal sealed class GetDepartmentByIdQueryHandler : IQueryHandler<GetDepartmentByIdQuery, DepartmentResponse>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public GetDepartmentByIdQueryHandler(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<Result<DepartmentResponse>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);

            if (department is null)
                return Result<DepartmentResponse>.Failure(DepartmentErrors.NotFound);

            var programResponses = department.Programs
                .Select(p => new ProgramResponse(p.Id, p.Name, p.RequiredCredits))
                .ToList();

            var response = new DepartmentResponse(
                department.Id,
                department.DepartmentName,
                department.Description,
                programResponses);

            return Result<DepartmentResponse>.Success(response);
        }
    }
}
