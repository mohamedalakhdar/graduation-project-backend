using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Registrations;

public sealed class GradeAppeal : Entity
{
    private GradeAppeal() { }

    private GradeAppeal(Guid id, Guid gradeId, string appealReason)
        : base(id)
    {
        GradeId = gradeId;
        AppealReason = appealReason;
        AppealDate = DateTime.UtcNow;
        Status = GradeAppealStatus.Pending;
    }

    public Guid GradeId { get; private set; }
    public Grade Grade { get; private set; } = null!;
    public string AppealReason { get; private set; }
    public DateTime AppealDate { get; private set; }
    public GradeAppealStatus Status { get; private set; }
    public string? ReviewNotes { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedDate { get; private set; }

    public static Result<GradeAppeal> Create(Guid gradeId, string appealReason)
    {
        if (gradeId == Guid.Empty)
            return Result<GradeAppeal>.Failure(Error.EmptyId("Grade"));

        if (string.IsNullOrWhiteSpace(appealReason))
            return Result<GradeAppeal>.Failure(GradeAppealErrors.EmptyReason);

        return Result<GradeAppeal>.Success(new GradeAppeal(Guid.NewGuid(), gradeId, appealReason.Trim()));
    }

    public Result Review(GradeAppealStatus newStatus, string reviewNotes, Guid reviewedBy)
    {
        if (Status != GradeAppealStatus.Pending)
            return Result.Failure(GradeAppealErrors.AlreadyReviewed);

        Status = newStatus;
        ReviewNotes = reviewNotes;
        ReviewedBy = reviewedBy;
        ReviewedDate = DateTime.UtcNow;

        return Result.Success();
    }
}
