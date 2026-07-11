using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;
using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Domain.Shared;
namespace CollegeControlSystem.Domain.CourseOfferings
{
    public sealed class CourseOffering:Entity
    {
        // Private constructor for EF Core
        private CourseOffering() { }

        private CourseOffering(
            Guid id,
            Guid courseId,
            Guid instructorId,
            Semester semester,
            int capacity) : base(id)
        {
            CourseId = courseId;
            InstructorId = instructorId;
            Semester = semester;
            Capacity = capacity;
            CurrentEnrolled = 0; // Starts empty
        }

        public Guid CourseId { get; private set; }
        public Course Course { get; private set; } // Navigation Property for transcript generation
        public Guid InstructorId { get; private set; } // Could link to faculty entity
        public Faculty Instructor { get; private set; } // Navigation Property
        public Semester Semester { get; private set; } // Value Object (Fall 2025)

        public int Capacity { get; private set; }
        public int CurrentEnrolled { get; private set; }
        public bool IsCancelled { get; private set; }

        // Derived property useful for UI
        public bool IsFull => CurrentEnrolled >= Capacity;
        public int AvailableSeats => Capacity - CurrentEnrolled;

        // Factory Method
        public static Result<CourseOffering> Create(
            Guid courseId,
            Guid instructorId,
            Semester semester,
            int capacity)
        {
            if (capacity <= 0)
                return Result<CourseOffering>.Failure(CourseOfferingErrors.InvalidCapacity);

            if (instructorId == Guid.Empty)
                return Result<CourseOffering>.Failure(CourseOfferingErrors.InstructorRequired);

            if (courseId == Guid.Empty)
                return Result<CourseOffering>.Failure(Error.EmptyId("Course"));

            return Result<CourseOffering>.Success(
                new CourseOffering(Guid.NewGuid(), courseId, instructorId, semester, capacity));
        }

        // --- Business Logic: Enrollment Management ---

        /// <summary>
        /// Attempts to reserve a spot for a student.
        /// Thread-safety note: This should be handled inside a transaction with 
        /// Optimistic Concurrency in the Persistence layer.
        /// </summary>
        public Result ReserveSeat()
        {
            if (IsCancelled)
                return Result.Failure(CourseOfferingErrors.OfferingCancelled);

            if (CurrentEnrolled >= Capacity)
            {
                return Result.Failure(CourseOfferingErrors.CapacityExceeded);
            }

            CurrentEnrolled++;
            return Result.Success();
        }

        /// <summary>
        /// Releases a spot (e.g., student drops the course).
        /// </summary>
        public void ReleaseSeat()
        {
            if (CurrentEnrolled > 0)
            {
                CurrentEnrolled--;
            }
        }

        // --- Business Logic: Admin Updates ---

        public Result UpdateCapacity(int newCapacity)
        {
            if (newCapacity <= 0)
                return Result.Failure(CourseOfferingErrors.InvalidCapacity);

            // Rule: You can't shrink the room if people are already inside!
            if (newCapacity < CurrentEnrolled)
                return Result.Failure(CourseOfferingErrors.CannotReduceCapacity);

            Capacity = newCapacity;
            return Result.Success();
        }

        public Result ChangeInstructor(Guid newInstructorId)
        {
            if (newInstructorId == Guid.Empty)
            {
                return Result.Failure(CourseOfferingErrors.InstructorRequired);
            }
                InstructorId = newInstructorId;
            return Result.Success();
        }

        public Result Cancel()
        {
            if (IsCancelled)
                return Result.Failure(CourseOfferingErrors.AlreadyCancelled);

            if (CurrentEnrolled > 0)
                return Result.Failure(CourseOfferingErrors.HasEnrolledStudents);

            IsCancelled = true;
            return Result.Success();
        }
    }
}
