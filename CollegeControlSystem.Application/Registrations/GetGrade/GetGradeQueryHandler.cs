using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;

namespace CollegeControlSystem.Application.Registrations.GetGrade;

internal sealed class GetGradeQueryHandler : IQueryHandler<GetGradeQuery, GetGradeResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetGradeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetGradeResponse>> Handle(GetGradeQuery request, CancellationToken cancellationToken)
    {
        var registration = await _unitOfWork.RegistrationRepository.GetByIdAsync(request.RegistrationId, cancellationToken);

        if (registration is null)
            return Result<GetGradeResponse>.Failure(RegistrationErrors.NotFound);

        var grade = await _unitOfWork.GradeRepository.GetByRegistrationIdAsync(request.RegistrationId);

        if (grade is null)
            return Result<GetGradeResponse>.Failure(GradeErrors.NotFound);

        var response = new GetGradeResponse(
            RegistrationId: grade.RegistrationId,
            SemesterWorkGrade: grade.SemesterWorkGrade,
            FinalGrade: grade.FinalGrade,
            TotalGrade: grade.TotalGrade,
            LetterGrade: grade.LetterGrade,
            GradePoints: grade.GradePoints,
            IsPassing: grade.IsPassing,
            IsRetake: registration.IsRetake
        );

        return Result<GetGradeResponse>.Success(response);
    }
}
