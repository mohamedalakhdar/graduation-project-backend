using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;

namespace CollegeControlSystem.Application.Registrations.ReviewAppeal;

internal sealed class ReviewAppealCommandHandler : ICommandHandler<ReviewAppealCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewAppealCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReviewAppealCommand request, CancellationToken cancellationToken)
    {
        var appeal = await _unitOfWork.GradeAppealRepository.GetByIdAsync(request.AppealId, cancellationToken);

        if (appeal is null)
            return Result.Failure(GradeAppealErrors.NotFound);

        var reviewResult = appeal.Review(request.Status, request.ReviewNotes, request.ReviewedBy);

        if (reviewResult.IsFailure)
            return Result.Failure(reviewResult.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
