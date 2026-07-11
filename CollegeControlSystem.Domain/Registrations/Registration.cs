using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Registrations.Events;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Domain.Registrations
{
    public sealed class Registration:Entity
    {
        private Registration() { }
        private Registration(Guid id, Guid studentId, Guid courseOfferingId, bool isRetake) : base(id)
        {
            StudentId = studentId;
            CourseOfferingId = courseOfferingId;
            Status = RegistrationStatus.Pending;
            IsRetake = isRetake;
            RegistrationDate = DateTime.UtcNow;
        }

        public Guid StudentId { get; private set; }
        public Student Student { get; private set; } // nav prop for advisor approval
        public Guid CourseOfferingId { get; private set; }
        // nav prop for transcript generation
        public CourseOffering CourseOffering { get; private set; }
        public RegistrationStatus Status { get; private set; }

        // We store this to help the Grading Logic know if it needs to cap points
        public bool IsRetake { get; private set; }

        public DateTime RegistrationDate { get; private set; }

        // nav prop for generating transcript
        public Grade Grade { get; private set; } 

        // factory meethod
        public static Result<Registration> Create(Guid studentId, Guid courseOfferingId, bool isRetake)
        {
            if (studentId == Guid.Empty)
                return Result<Registration>.Failure(Error.EmptyId("Student"));

            if (courseOfferingId == Guid.Empty)
                return Result<Registration>.Failure(Error.EmptyId("CourseOffering"));

            var registration = new Registration(Guid.NewGuid(), studentId, courseOfferingId, isRetake);
            //registration.RaiseDomainEvent(new RegistrationCreatedDomainEvent(registration.Id));

            return Result<Registration>.Success(registration);
        }

        /// <summary>
        /// Called by Advisor when they click "Approve".
        /// </summary>
        public Result Approve()
        {
            if (Status != RegistrationStatus.Pending)
                return Result.Failure(RegistrationErrors.NotPending);

            Status = RegistrationStatus.Approved;
            return Result.Success();
        }

        /// <summary>
        /// Called by Student during Drop period.
        /// </summary>
        public Result Drop()
        {
            if (Status == RegistrationStatus.Completed)
                return Result.Failure(RegistrationErrors.AlreadyCompleted);

            Status = RegistrationStatus.Dropped;
            return Result.Success();
        }

        /// <summary>
        /// Called by Instructor/System when a Grade is assigned.
        /// </summary>
        public Result MarkAsCompleted()
        {
            if (Status == RegistrationStatus.Dropped || Status == RegistrationStatus.Withdrawn)
                return Result.Failure(RegistrationErrors.AlreadyDropped);

            Status = RegistrationStatus.Completed;
            return Result.Success();
        }

        /// <summary>
        /// Called when student withdraws late in the semester (gets a 'W').
        /// </summary>
        public Result Withdraw()
        {
            if (Status == RegistrationStatus.Completed)
                return Result.Failure(RegistrationErrors.AlreadyCompleted);

            Status = RegistrationStatus.Withdrawn;
            return Result.Success();
        }
    }
}
