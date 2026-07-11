using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.Students.GetTranscript
{
    public record GetTranscriptQuery(Guid StudentId) : IQuery<TranscriptResponse>;
}
