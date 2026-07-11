using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.ControlEngine.GetWarnings
{
    public sealed record GetAcademicWarningsQuery() : IQuery<List<AcademicWarningResponse>>;
}
