using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.ControlEngine.RunEngine
{
    public sealed record RunControlEngineCommand(string Term, int Year) : ICommand<ControlEngineSummaryResponse>;
}
