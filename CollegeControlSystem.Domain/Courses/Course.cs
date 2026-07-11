using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Courses
{
    public sealed class Course:Entity
    {
        private Course(){ }

        private Course(
            Guid id,
            Guid departmentId,
            CourseCode code,
            string title,
            string description,
            int credits,
            int lectureHours,
            int labHours) : base(id)
        {
            DepartmentId = departmentId;
            Code = code;
            Title = title;
            Description = description;
            Credits = credits;
            LectureHours = lectureHours;
            LabHours = labHours;
        }

        public Guid DepartmentId { get; private set; }
        public CourseCode Code { get; private set; } 
        public string Title { get; private set; }
        public string? Description { get; private set; }

        public int Credits { get; private set; }
        public int LectureHours { get; private set; }
        public int LabHours { get; private set; }

        // level ??

        // nav property
        public List<CoursePrerequisite> Prerequisites { get; private set; } = new();

        public static Result<Course> Create(
            Guid departmentId,
            string codeString,
            string title,
            string description,
            int credits,
            int lectureHours,
            int labHours)
        {

            if (departmentId == Guid.Empty)
                return Result<Course>.Failure(Error.EmptyId("Department"));

            if (lectureHours < 0 || labHours < 0)
                return Result<Course>.Failure(CourseErrors.InvalidHours);

            var codeResult = CourseCode.Create(codeString);
            if (codeResult.IsFailure) return Result<Course>.Failure(codeResult.Error);

            if (credits <= 0) return Result<Course>.Failure(CourseErrors.CreditsInvalid);

            var course = new Course(
                Guid.NewGuid(),
                departmentId,
                codeResult.Value,
                title,
                description,
                credits,
                lectureHours,
                labHours);

            return Result<Course>.Success(course);
        }

        public Result AddPrerequisite(Guid prerequisiteCourseId)
        {
            //if (prerequisiteCourseId == Id)
            //    return Result.Failure(CourseErrors.PrerequisiteCycle);

            if (Prerequisites.Any(p => p.PrerequisiteCourseId == prerequisiteCourseId))
                return Result.Failure(CourseErrors.PrerequisiteDuplicate);

            var result=CoursePrerequisite.Create(Id, prerequisiteCourseId);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            Prerequisites.Add(result.Value);
            return Result.Success();
        }

        public Result RemovePrerequisite(Guid prerequisiteCourseId)
        {
            var match = Prerequisites.FirstOrDefault(p => p.PrerequisiteCourseId == prerequisiteCourseId);
            if (match != null)
            {
                Prerequisites.Remove(match);
            }
            return Result.Success();
        }

        public Result Update(
            Guid departmentId,
            string title,
            string description,
            int credits,
            int lectureHours,
            int labHours)
        {
            if (departmentId == Guid.Empty)
                return Result.Failure(Error.EmptyId("Department"));

            if (lectureHours < 0 || labHours < 0)
                return Result.Failure(CourseErrors.InvalidHours);

            if (credits <= 0)
                return Result.Failure(CourseErrors.CreditsInvalid);

            DepartmentId = departmentId;
            Title = title;
            Description = description;
            Credits = credits;
            LectureHours = lectureHours;
            LabHours = labHours;

            return Result.Success();
        }
    }
}
