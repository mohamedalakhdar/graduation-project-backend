using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;

namespace CollegeControlSystem.Application.Faculties.GetInstructorCourses
{
    internal sealed class GetInstructorCoursesQueryHandler : IQueryHandler<GetInstructorCoursesQuery, GetInstructorCoursesQueryResponse>
    {
        private readonly IUnitOfWork _uow;


        public GetInstructorCoursesQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<GetInstructorCoursesQueryResponse>> Handle(GetInstructorCoursesQuery request, CancellationToken cancellationToken)
        {
            var offerings = await _uow.CourseOfferingRepository.GetByInstructorIdAsync(request.InstructorId, cancellationToken);


            var courses = offerings.Select(o => new InstructorCourseResponse(
               o.Id,
               o.Course?.Code?.Value ?? "N/A", // Assuming Value Object for Code
               o.Course?.Title ?? "N/A",
               o.Semester.Term.ToString(),     // Assuming Value Object for Semester
               o.Semester.Year,
               o.CurrentEnrolled,
               o.Capacity
           )).ToList();

            var response = new GetInstructorCoursesQueryResponse
            {
                InstructorId = request.InstructorId,
                Courses = courses
            };

            return Result<GetInstructorCoursesQueryResponse>.Success(response);
        }
    }
}