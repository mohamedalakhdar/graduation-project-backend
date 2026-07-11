using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Registrations;

public static class GradeAppealErrors
{
    public static readonly Error EmptyReason = new(
        "GradeAppeal.EmptyReason",
        "Appeal reason cannot be empty.");

    public static readonly Error NotFound = new(
        "GradeAppeal.NotFound",
        "Grade appeal not found.");

    public static readonly Error AlreadyAppealed = new(
        "GradeAppeal.AlreadyAppealed",
        "A grade can only be appealed once.");

    public static readonly Error NoGradeToAppeal = new(
        "GradeAppeal.NoGradeToAppeal",
        "Cannot appeal a grade that does not exist.");

    public static readonly Error AlreadyReviewed = new(
        "GradeAppeal.AlreadyReviewed",
        "This appeal has already been reviewed.");
}
