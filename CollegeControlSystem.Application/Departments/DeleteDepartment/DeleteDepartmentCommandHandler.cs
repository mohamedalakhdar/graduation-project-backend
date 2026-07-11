using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;

namespace CollegeControlSystem.Application.Departments.DeleteDepartment
{
    internal sealed class DeleteDepartmentCommandHandler : ICommandHandler<DeleteDepartmentCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);

            if (department is null)
                return Result.Failure(DepartmentErrors.NotFound);

            // Integrity Check: Cannot delete if it has programs
            if (department.Programs.Any())
                return Result.Failure(DepartmentErrors.HasPrograms);

            // Integrity Check: Cannot delete if it has faculty members
            var faculties = await _unitOfWork.FacultyRepository.GetByDepartmentIdAsync(request.DepartmentId, cancellationToken);
            if (faculties.Any())
                return Result.Failure(DepartmentErrors.HasFaculties);

            // Integrity Check: Cannot delete if it has courses
            var courses = await _unitOfWork.CourseRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
            if (courses.Any())
                return Result.Failure(DepartmentErrors.HasCourses);

            _unitOfWork.DepartmentRepository.Delete(department);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
