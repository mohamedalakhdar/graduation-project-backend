using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.Faculties.GetFacultyById
{
    public sealed record GetFacultyByIdQuery(Guid Id) : IQuery<GetFacultyByIdQueryResponse>;
}
