using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;

namespace CollegeControlSystem.Domain.Shared
{
    public sealed record GradeScale
    {
        // Private constructor enforces use of Factory method
        private GradeScale(string letter, decimal points, bool isPassing)
        {
            Letter = letter;
            Points = points;
            IsPassing = isPassing;
        }

        public string Letter { get; }
        public decimal Points { get; }
        public bool IsPassing { get; }


        public static readonly GradeScale APlus = new("A+", 4.0m, true);
        public static readonly GradeScale A = new("A", 4.0m, true);
        public static readonly GradeScale AMinus = new("A-", 3.7m, true);
        public static readonly GradeScale BPlus = new("B+", 3.3m, true);
        public static readonly GradeScale B = new("B", 3.0m, true);
        public static readonly GradeScale BMinus = new("B-", 2.7m, true);
        public static readonly GradeScale CPlus = new("C+", 2.3m, true);
        public static readonly GradeScale C = new("C", 2.0m, true);
        public static readonly GradeScale CMinus = new("C-", 1.7m, true);
        public static readonly GradeScale DPlus = new("D+", 1.3m, true);
        public static readonly GradeScale D = new("D", 1.0m, true);
        public static readonly GradeScale F = new("F", 0.0m, false);

        public static readonly GradeScale W = new("W", 0.0m, false); // Withdrawn (Excluded from GPA)
        public static readonly GradeScale I = new("I", 0.0m, false); // Incomplete
        public static readonly GradeScale P = new("P", 0.0m, true);  // Pass (Excluded from GPA)

        /// <summary>
        /// Creates a GradeScale Value Object from a letter string.
        /// Strictly validates against the defined list.
        /// </summary>
        public static Result<GradeScale> FromLetter(string letter)
        {
            if (string.IsNullOrWhiteSpace(letter))
                return Result<GradeScale>.Failure(GradeErrors.EmptyLetter);

            return letter.ToUpper().Trim() switch
            {
                "A+" => Result<GradeScale>.Success(APlus),
                "A" => Result<GradeScale>.Success(A),
                "A-" => Result<GradeScale>.Success(AMinus),
                "B+" => Result<GradeScale>.Success(BPlus),
                "B" => Result<GradeScale>.Success(B),
                "B-" => Result<GradeScale>.Success(BMinus),
                "C+" => Result<GradeScale>.Success(CPlus),
                "C" => Result<GradeScale>.Success(C),
                "C-" => Result<GradeScale>.Success(CMinus),
                "D+" => Result<GradeScale>.Success(DPlus),
                "D" => Result<GradeScale>.Success(D),
                "F" => Result<GradeScale>.Success(F),
                "W" => Result<GradeScale>.Success(W),
                "I" => Result<GradeScale>.Success(I),
                "P" => Result<GradeScale>.Success(P),
                _ => Result<GradeScale>.Failure(GradeErrors.InvalidLetter(letter))
            };
        }

    public static GradeScale FromScore(decimal totalScore)
        {
            return totalScore switch
            {
                >= 97m => APlus,
                >= 93m => A,
                >= 89m => AMinus,
                >= 84m => BPlus,
                >= 80m => B,
                >= 76m => BMinus,
                >= 73m => CPlus,
                >= 70m => C,
                >= 67m => CMinus,
                >= 64m => DPlus,
                >= 60m => D,
                _ => F
            };
        }
        
        /// <summary>
        /// Implements Article 28: Course Retake Policy.
        /// If a student retakes a compulsory course they previously failed,
        /// </summary>
        public GradeScale ApplyRetakeCap()
        {
            // If the earned GradeScale points exceed 3.3 (B+), cap it.
            if (Points > 3.3m)
            {
                return BPlus;
            }
            return this;
        }

        /// <summary>
        /// Returns true if the GradeScale should be included in GPA calculations.
        /// </summary>
        public bool IsIncludedInGpa()
        {
            return this != W && this != I && this != P;
        }
    }
};
