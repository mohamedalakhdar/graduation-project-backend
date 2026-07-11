using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.ControlEngine.GetGraduates
{
    public sealed record GetGraduationCandidatesQuery() : IQuery<List<GraduationCandidateResponse>>;
}
