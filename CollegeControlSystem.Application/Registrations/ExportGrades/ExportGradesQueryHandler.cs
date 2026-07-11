using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Registrations;
using System.Text;


namespace CollegeControlSystem.Application.Registrations.ExportGrades
{
    internal sealed class ExportGradesQueryHandler : IQueryHandler<ExportGradesQuery, FileResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExportGradesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<FileResponse>> Handle(ExportGradesQuery request, CancellationToken cancellationToken)
        {
            // 1. Verify Offering exists
            var offering = await _unitOfWork.CourseOfferingRepository.GetByIdAsync(request.OfferingId, cancellationToken);
            if (offering is null)
            {
                return Result<FileResponse>.Failure(CourseOfferingErrors.OfferingNotFound);
            }

            // 2. Security Check: Only the assigned instructor or an Admin can export these grades
            if (!request.IsAdmin && offering.InstructorId != request.RequestingUserId)
            {
                return Result<FileResponse>.Failure(GradeErrors.Unauthorized);
            }

            // 3. Fetch Registrations (Must Include Student and Grade navigation properties in the Repository)
            var registrations = await _unitOfWork.RegistrationRepository.GetByOfferingIdAsync(request.OfferingId, cancellationToken);

            // 4. Build CSV Content
            var sb = new StringBuilder();

            // CSV Header
            sb.AppendLine("Academic Number,Student Name,Status,Is Retake,Semester Work,Final Exam,Total,Letter Grade,Points");

            // Sort by Academic Number for a clean roster
            foreach (var reg in registrations.OrderBy(r => r.Student.AcademicNumber))
            {
                var academicNum = reg.Student.AcademicNumber;
                var studentName = reg.Student.StudentName;
                var status = reg.Status.ToString();
                var isRetake = reg.IsRetake ? "Yes" : "No";

                // Handle potential nulls if the student hasn't been graded yet
                var sw = reg.Grade?.SemesterWorkGrade.ToString("0.##") ?? "N/A";
                var final = reg.Grade?.FinalGrade.ToString("0.##") ?? "N/A";
                var total = reg.Grade?.TotalGrade.ToString("0.##") ?? "N/A";
                var letter = reg.Grade?.LetterGrade ?? "N/A";
                var points = reg.Grade?.GradePoints.ToString("0.##") ?? "N/A";

                // Escape commas in student names to prevent CSV format breaking
                if (studentName.Contains(','))
                {
                    studentName = $"\"{studentName}\"";
                }

                sb.AppendLine($"{academicNum},{studentName},{status},{isRetake},{sw},{final},{total},{letter},{points}");
            }

            // 5. Apply UTF-8 BOM (Byte Order Mark) 
            // VERY IMPORTANT: This ensures Arabic characters in 'StudentName' display correctly in MS Excel.
            byte[] utf8Bom = { 0xEF, 0xBB, 0xBF };
            byte[] csvBytes = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] finalBytes = utf8Bom.Concat(csvBytes).ToArray();

            // 6. Format the response
            string safeCourseTitle = offering.Course?.Title?.Replace(" ", "_") ?? "Course";
            var fileResponse = new FileResponse(
                FileName: $"{safeCourseTitle}_Grades_{offering.Semester.Term}_{offering.Semester.Year}.csv",
                ContentType: "text/csv",
                Content: finalBytes
            );

            return Result<FileResponse>.Success(fileResponse);
        }
    }
}
