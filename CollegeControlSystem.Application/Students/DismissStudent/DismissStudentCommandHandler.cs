using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.DismissStudent
{
    internal sealed class DismissStudentCommandHandler : ICommandHandler<DismissStudentCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DismissStudentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DismissStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.StudentId, cancellationToken);

            if (student is null)
                return Result.Failure(StudentErrors.StudentNotFound);

            if (student.AcademicStatus == AcademicStatus.Dismissed)
                return Result.Failure(StudentErrors.AlreadyDismissed);

            student.Dismiss();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
