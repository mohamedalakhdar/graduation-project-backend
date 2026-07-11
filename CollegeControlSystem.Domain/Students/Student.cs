using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students.Events;

namespace CollegeControlSystem.Domain.Students
{
    public sealed class Student : Entity
    {
        // Private constructor for EF Core
        private Student() { }

        private Student(
            Guid id,
            string studentName,
            string academicNumber,
            Guid programId,
            string appUserId ,
             string nationalId)
            : base(id)
        {
            AcademicNumber = academicNumber;
            ProgramId = programId;
            AcademicStatus = AcademicStatus.GoodStanding;
            AcademicLevel= AcademicLevel.Freshman;
            CGPA = 0.00m;
            ConsecutiveWarnings = 0;
            AppUserId= appUserId;
            StudentName = studentName;
            NationalId = nationalId;

        }

        public string StudentName { get; private set; }
        public string AcademicNumber { get; private set; }
        public Guid ProgramId { get; private set; }
        public Program Program { get; private set; }
        public Guid? AdvisorId { get; private set; }
        public string NationalId { get; private set; }

        public decimal CGPA { get; private set; }
        public AcademicStatus AcademicStatus { get; private set; }
        public AcademicLevel AcademicLevel { get; private set; }
        public int ConsecutiveWarnings { get; private set; }

        public string AppUserId { get; private set; }
        public AppUser AppUser { get; private set; }

        // Navigational Property for transcript generation
        public List<Registration> Registrations { get; private set; } = new List<Registration>();
        // Factory Method
        public static Result<Student> Create(
            string studentName,
            string academicNumber,
            Guid programId,
            string appUserId,
            string nationalId)
        {
            // Validation logic
            if (string.IsNullOrWhiteSpace(studentName))
                return Result<Student>.Failure(Error.NullValue);  // check it later

            if (string.IsNullOrWhiteSpace(appUserId))
                return Result<Student>.Failure(Error.EmptyId("User"));

            if (string.IsNullOrWhiteSpace(academicNumber))
                return Result<Student>.Failure(StudentErrors.AcademicNumberRequired);

            if (string.IsNullOrWhiteSpace(nationalId))
                return Result<Student>.Failure(StudentErrors.NationalIdRequired);

            if (programId == Guid.Empty)
                return Result<Student>.Failure(StudentErrors.ProgramRequired);

            // National ID validation for Egyptian pattern (14 digits)
            if (nationalId.Length != 14 || !nationalId.All(char.IsDigit))
                return Result<Student>.Failure(StudentErrors.NationalIdInvalid);

            if (programId == Guid.Empty)
                return Result<Student>.Failure(Error.EmptyId("Program"));


            // Create the student object
            var student = new Student(
                Guid.NewGuid(),
                studentName,
                academicNumber,
                programId,
                appUserId,
                nationalId
            );

            return Result<Student>.Success(student);
        }

        // Business Logic: Article 12 (Academic Load)
        // max credit hours a student can register in the semester based on their academic status and CGPA.
        public int GetMaxAllowedCreditHours(bool isGraduatingSemester = false)
        {
            // "Warning Status -> Max 14 hours"
            if (AcademicStatus == AcademicStatus.AcademicWarning)
                return StudentConstants.MaxCreditsWarning;

            int maxCredits = StudentConstants.MaxCreditsWarning; // Fallback

            // "CGPA >= 3.00 -> Max 21 hours"
            if (CGPA >= 3.00m)
                maxCredits = StudentConstants.MaxCreditsHighCgpa ;
            // "2.00 <= CGPA < 3.00 -> Max 18 hours"
            else if (CGPA >= 2.00m)
                maxCredits = StudentConstants.MaxCreditsLowCgpa;

            // "Exception: Graduating student can register 1 extra course"

            // Assuming average course is 3 credits.
            if (isGraduatingSemester)
                maxCredits += StudentConstants.ExtraCreditsGraduating;

            return maxCredits;
        }

        // Called by the "Control Engine" at the end of every semester.
        public void ProcessSemesterResults(decimal sgpa, decimal newCgpa)
        {
            CGPA = newCgpa;

            if (sgpa < 2.00m)
            {
                // "Student receives academic warning if SGPA < 2.00"
                AcademicStatus = AcademicStatus.AcademicWarning;
                ConsecutiveWarnings++;
            }
            else
            {
                // Reset if good performance
                AcademicStatus = AcademicStatus.GoodStanding;
                ConsecutiveWarnings = 0;
            }

            // "Dismissed if 4 consecutive warnings" 
            if (ConsecutiveWarnings >= 4)
            {
                AcademicStatus = AcademicStatus.Dismissed;
                // Potentially raise Domain Event: StudentDismissed so other systems can react (e.g., notify advisor, notify student )
                RaiseDomainEvent(new StudentDismissedDomainEvent(
                            StudentId: this.Id,
                            AcademicNumber: this.AcademicNumber,
                            Reason: "Exceeded limit of 4 consecutive academic warnings.",
                            ConsecutiveWarningsCount: this.ConsecutiveWarnings,
                            OccurredOn: DateTime.UtcNow
                        ));
            }
        }

        public void ProcessSemesterResults(decimal semesterGpa, decimal newCumulativeGpa, int totalCreditsCompleted)
        {
            CGPA = newCumulativeGpa;

            UpdateAcademicLevel(totalCreditsCompleted);

            if (semesterGpa < 2.00m)
            {
                AcademicStatus = AcademicStatus.AcademicWarning;
                ConsecutiveWarnings++;
            }
            else
            {
                AcademicStatus = AcademicStatus.GoodStanding;
                ConsecutiveWarnings = 0;
            }

            if (ConsecutiveWarnings >= 4)
            {
                AcademicStatus = AcademicStatus.Dismissed;
                RaiseDomainEvent(new StudentDismissedDomainEvent(
                    StudentId: this.Id,
                    AcademicNumber: this.AcademicNumber,
                    Reason: "Exceeded limit of 4 consecutive academic warnings.",
                    ConsecutiveWarningsCount: this.ConsecutiveWarnings,
                    OccurredOn: DateTime.UtcNow
                ));
            }
        }


        public Result AssignAdvisor(Guid advisorId)
        {
            if (advisorId == Guid.Empty)
                return Result.Failure(StudentErrors.InvalidAdvisor);
            AdvisorId = advisorId;
            return Result.Success();
        }

        private Result UpdateAcademicLevel(int credits)
        {
            if (Program is null)
                return Result.Failure(StudentErrors.ProgramRequired);

            int step = Program.RequiredCredits / 5;

            AcademicLevel = credits switch
            {
                _ when credits < step => AcademicLevel.Freshman,
                _ when credits < step * 2 => AcademicLevel.Sophomore,
                _ when credits < step * 3 => AcademicLevel.Junior,
                _ when credits < step * 4 => AcademicLevel.Senior1,
                _ => AcademicLevel.Senior2
            };

            return Result.Success();
        }

        public Result UpdatePersonalDetails(string newFullName, string newNationalId)
        {
            if (!string.IsNullOrWhiteSpace(newFullName))
            {
                StudentName = newFullName;
            }
            if (!string.IsNullOrWhiteSpace(newNationalId))
            {
                if (newNationalId.Length == 14 && newNationalId.All(char.IsDigit))
                {
                    NationalId = newNationalId;
                }
                else
                {
                    return Result<Student>.Failure(StudentErrors.NationalIdInvalid);
                }
            }
            return Result.Success();
        }

        public void Dismiss()
        {
            AcademicStatus = AcademicStatus.Dismissed;
            RaiseDomainEvent(new StudentDismissedDomainEvent(
                StudentId: this.Id,
                AcademicNumber: this.AcademicNumber,
                Reason: "Manually dismissed by administrator.",
                ConsecutiveWarningsCount: this.ConsecutiveWarnings,
                OccurredOn: DateTime.UtcNow
            ));
        }

        public Result ChangeProgram(Guid newProgramId)
        {
            if (newProgramId == Guid.Empty)
                return Result.Failure(StudentErrors.ProgramRequired);

            if (AcademicStatus == AcademicStatus.Dismissed)
                return Result.Failure(StudentErrors.CannotChangeProgramForDismissedStudent);

            ProgramId = newProgramId;
            Program = null!;
            return Result.Success();
        }
    }
}
