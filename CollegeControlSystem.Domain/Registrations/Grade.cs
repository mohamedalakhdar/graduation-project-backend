using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Shared;

namespace CollegeControlSystem.Domain.Registrations
{
    public sealed class Grade:Entity
    {
        private Grade() { }

        private Grade(Guid id, Guid registrationId, decimal semesterWork, decimal finalExam, bool isRetake)
            : base(id)
        {
            RegistrationId = registrationId;
            CalculateAndSet(semesterWork, finalExam, isRetake);
        }

        // Database Columns
        public Guid RegistrationId { get; private set; }
        public decimal SemesterWorkGrade { get; private set; }
        public decimal FinalGrade { get; private set; }
        public decimal TotalGrade { get; private set; }

        // These are calculated based on your Value Object logic
        public string LetterGrade { get; private set; }
        public decimal GradePoints { get; private set; }

        public Registration Registration { get; private set; } = null!;

        public bool IsPassing
        {
            get
            {
                var scaleResult = GradeScale.FromLetter(LetterGrade);
                // If the letter is valid, return its IsPassing status. Otherwise false.
                return scaleResult.IsSuccess && scaleResult.Value.IsPassing;
            }
        }

        // for updating grades and applying retake logic
        public Result CalculateAndSet(decimal semesterWork, decimal finalExam, bool isRetake)
        {

            if (semesterWork < 0 || finalExam < 0)
                return Result.Failure(GradeErrors.NegativeScore);
            if (semesterWork > 100 || finalExam > 100 || semesterWork+finalExam >100)
                return Result.Failure(GradeErrors.ExceededScore);



            SemesterWorkGrade = semesterWork;
            FinalGrade = finalExam;
            TotalGrade = semesterWork + finalExam;

            // 1. Get the "Raw" GradeScale based on score
            // Example: Score 95 -> returns A (4.0)
            GradeScale scale = GradeScale.FromScore(TotalGrade);

            // 2. Apply Retake Logic (The Business Logic you asked about)
            if (isRetake)
            {
                // Example: If scale was A (4.0), it becomes B+ (3.3)
                scale = scale.ApplyRetakeCap();
            }

            // 3. Save the results to the Entity properties
            LetterGrade = scale.Letter;
            GradePoints = scale.Points;

            return Result.Success();
        }

        public static Result<Grade> Create(Guid registrationId, decimal semesterWork, decimal finalExam, bool isRetake)
        {
            if (semesterWork < 0 || finalExam < 0)
                return Result<Grade>.Failure(GradeErrors.NegativeScore);
            if (semesterWork > 100 || finalExam > 100 || semesterWork + finalExam > 100)
                return Result<Grade>.Failure(GradeErrors.ExceededScore);

            return Result<Grade>.Success( new Grade(Guid.NewGuid(), registrationId, semesterWork, finalExam, isRetake));
        }
    }
}
