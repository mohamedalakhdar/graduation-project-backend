using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Students.GetTranscript
{
    internal sealed class GetTranscriptQueryHandler : IQueryHandler<GetTranscriptQuery, TranscriptResponse>
    {
        private readonly IStudentRepository _studentRepository;

        public GetTranscriptQueryHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<Result<TranscriptResponse>> Handle(GetTranscriptQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch Student with deep includes (Registrations -> Offerings -> Course)
            var student = await _studentRepository.GetTranscriptDataAsync(request.StudentId, cancellationToken);

            if (student is null) return Result<TranscriptResponse>.Failure(StudentErrors.StudentNotFound);

            return Result<TranscriptResponse>.Success(MapToDto(student));
        }

        private static TranscriptResponse MapToDto(Student student)
        {
            var semesters = student.Registrations
                .GroupBy(r => new { r.CourseOffering.Semester.Term, r.CourseOffering.Semester.Year })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Term)
                .Select(g =>
                {
                    var gradedCourses = g.Where(r => r.Grade is not null).ToList();
                    int totalCredits = gradedCourses.Sum(r => r.CourseOffering.Course.Credits);
                    decimal sgpa = totalCredits > 0
                        ? Math.Round(
                            gradedCourses.Sum(r => r.CourseOffering.Course.Credits * r.Grade!.GradePoints)
                            / totalCredits, 2)
                        : 0.0m;

                    return new SemesterTranscriptDto(
                        g.Key.Term,
                        g.Key.Year,
                        sgpa,
                        g.Select(r => new CourseTranscriptDto(
                            r.CourseOffering.Course.Code.Value,
                            r.CourseOffering.Course.Title,
                            r.CourseOffering.Course.Credits,
                            r.Grade is null ? "IP" : r.Grade.LetterGrade,
                            r.Grade is null ? 0.0m : r.Grade.GradePoints
                        )).ToList()
                    );
                }).ToList();

            return new TranscriptResponse(
                student.StudentName,
                student.AcademicNumber,
                student.Program?.Name ?? "N/A",
                student.CGPA,
                semesters
            );
        }
    }


}
