using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.ControlEngine.GetStatistics
{
    public sealed record GetStatisticsQuery() : IQuery<StatisticsResponse>;
}
