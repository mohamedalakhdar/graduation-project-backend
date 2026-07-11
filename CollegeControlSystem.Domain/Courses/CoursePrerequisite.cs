using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Courses
{
    public sealed class CoursePrerequisite:Entity
    {
        private CoursePrerequisite(Guid courseId, Guid prerequisiteCourseId)
        {
            CourseId = courseId;
            PrerequisiteCourseId = prerequisiteCourseId;
        }

        // Composite Key in DB (PK, FK)
        public Guid CourseId { get; private set; }

        // Composite Key in DB (PK, FK)
        public Guid PrerequisiteCourseId { get; private set; }

        public Course Course { get; private set; } = null!;
        public Course PrerequisiteCourse { get; private set; } = null!;

        // Factory Method
        public static Result<CoursePrerequisite> Create(Guid courseId, Guid prerequisiteCourseId)
        {
            // 1. A course cannot be a prerequisite of itself
            if (courseId == prerequisiteCourseId)
                return Result<CoursePrerequisite>.Failure(CourseErrors.PrerequisiteCycle);

            // 2. Create Instance
            var prerequisite = new CoursePrerequisite(courseId, prerequisiteCourseId);
            return Result<CoursePrerequisite>.Success(prerequisite);
        }
    }
}
