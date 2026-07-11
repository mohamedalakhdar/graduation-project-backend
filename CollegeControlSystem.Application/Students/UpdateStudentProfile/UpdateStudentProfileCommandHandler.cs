using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Students;
namespace CollegeControlSystem.Application.Students.UpdateStudentProfile
{
    internal sealed class UpdateStudentProfileCommandHandler : ICommandHandler<UpdateStudentProfileCommand>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentProfileCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateStudentProfileCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student is null) return Result.Failure(StudentErrors.StudentNotFound);

            var result=student.UpdatePersonalDetails(request.NewFullName, request.NewNationalId);
            if (result.IsFailure) return Result.Failure(result.Error);
            _studentRepository.Update(student);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
