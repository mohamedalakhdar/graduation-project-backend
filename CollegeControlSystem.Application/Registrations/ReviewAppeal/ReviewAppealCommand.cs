using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Registrations;

namespace CollegeControlSystem.Application.Registrations.ReviewAppeal;

public sealed record ReviewAppealCommand(Guid AppealId, GradeAppealStatus Status, string ReviewNotes, Guid ReviewedBy) : ICommand;
