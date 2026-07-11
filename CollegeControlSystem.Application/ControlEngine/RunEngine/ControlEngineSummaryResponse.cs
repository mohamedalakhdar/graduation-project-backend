namespace CollegeControlSystem.Application.ControlEngine.RunEngine
{
    public sealed record ControlEngineSummaryResponse(
            int TotalProcessed,
            int NewWarningsIssued,
            int StudentsDismissed,
            int StudentsReturnedToGoodStanding
        );
}
