using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.GetProgramById
{
    internal sealed class GetProgramByIdQueryHandler : IQueryHandler<GetProgramByIdQuery, ProgramDetailResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProgramByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProgramDetailResponse>> Handle(GetProgramByIdQuery request, CancellationToken cancellationToken)
        {
            var program = await _unitOfWork.DepartmentRepository.GetProgramByIdAsync(request.ProgramId, cancellationToken);

            if (program is null)
            {
                return Result<ProgramDetailResponse>.Failure(DepartmentErrors.ProgramNotFound);
            }

            //if (program.DepartmentId != request.DepartmentId)
            //{
            //    return Result<ProgramDetailResponse>.Failure(DepartmentErrors.ProgramNotFound);
            //}

            var response = new ProgramDetailResponse(
                program.Id,
                program.Name,
                program.RequiredCredits,
                program.DepartmentId,
                program.Department.DepartmentName
            );

            return Result<ProgramDetailResponse>.Success(response);
        }
    }
}
