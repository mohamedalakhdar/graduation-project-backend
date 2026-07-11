using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Students;
namespace CollegeControlSystem.Application.Students.AssignAdvisor
{
    internal sealed class AssignAdvisorCommandHandler : ICommandHandler<AssignAdvisorCommand>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignAdvisorCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AssignAdvisorCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student is null) return Result.Failure(StudentErrors.StudentNotFound);

            var result = student.AssignAdvisor(request.AdvisorId);

            if (result.IsFailure) return result;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}