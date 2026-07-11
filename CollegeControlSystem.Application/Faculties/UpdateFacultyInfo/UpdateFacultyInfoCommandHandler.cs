using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;
namespace CollegeControlSystem.Application.Faculties.UpdateFacultyInfo
{
    internal sealed class UpdateFacultyInfoCommandHandler : ICommandHandler<UpdateFacultyInfoCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFacultyInfoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateFacultyInfoCommand request, CancellationToken cancellationToken)
        {
            var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.FacultyId, cancellationToken);

            if (faculty is null)
            {
                return Result.Failure(FacultyErrors.NotFound);
            }

            var result = faculty.UpdateDegree(request.NewDegree);


            if (result.IsFailure) return result;

            //_unitOfWork.FacultyRepository.Update(faculty);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
