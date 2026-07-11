using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;

namespace CollegeControlSystem.Application.Registrations.AppealGrade;

internal sealed class AppealGradeCommandHandler : ICommandHandler<AppealGradeCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public AppealGradeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AppealGradeCommand request, CancellationToken cancellationToken)
    {
        var registration = await _unitOfWork.RegistrationRepository.GetByIdAsync(request.RegistrationId, cancellationToken);

        if (registration is null)
            return Result<Guid>.Failure(RegistrationErrors.NotFound);

        var grade = await _unitOfWork.GradeRepository.GetByRegistrationIdAsync(request.RegistrationId);

        if (grade is null)
            return Result<Guid>.Failure(GradeAppealErrors.NoGradeToAppeal);

        var existingAppeal = await _unitOfWork.GradeAppealRepository.GetByGradeIdAsync(grade.Id, cancellationToken);

        if (existingAppeal is not null && existingAppeal.Status == GradeAppealStatus.Pending)
            return Result<Guid>.Failure(GradeAppealErrors.AlreadyAppealed);

        var appealResult = GradeAppeal.Create(grade.Id, request.Reason);

        if (appealResult.IsFailure)
            return Result<Guid>.Failure(appealResult.Error);

        await _unitOfWork.GradeAppealRepository.AddAsync(appealResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(appealResult.Value.Id);
    }
}
