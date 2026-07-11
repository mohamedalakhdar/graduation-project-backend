using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Shared
{
    public  record Semester:IComparable<Semester>
    {
        public string Term { get; }
        public int Year { get; }

        // Private constructor for EF Core and Factory
        private Semester(string term, int year)
        {
            Term = term;
            Year = year;
        }

        // Valid terms based on typical university schedules
        private static readonly string[] ValidTerms = { "Fall", "Spring", "Summer" };

        public static Result<Semester> Create(string term, int year)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Result<Semester>.Failure(SemsterErrors.EmptyTerm);

            // Normalize term (Title Case)
            var normalizedTerm = char.ToUpper(term[0]) + term.Substring(1).ToLower();

            if (!ValidTerms.Contains(normalizedTerm))
                return Result<Semester>.Failure(SemsterErrors.InvalidTerm);

            if (year < 2020 || year > 2100)
                return Result<Semester>.Failure(SemsterErrors.InvalidYear);

            return Result<Semester>.Success(new Semester(normalizedTerm, year));
        }

        // Helper for easier code readability
        public bool IsSummer => Term == "Summer";

        // Comparable implementation allows sorting (e.g., transcripts)
        public int CompareTo(Semester? other)
        {
            if (other is null) return 1;

            if (Year != other.Year)
                return Year.CompareTo(other.Year);

            // Custom order: Spring < Summer < Fall
            return GetTermOrder(Term).CompareTo(GetTermOrder(other.Term));
        }

        private static int GetTermOrder(string term) => term switch
        {
            "Spring" => 1,
            "Summer" => 2,
            "Fall" => 3,
            _ => 0
        };
    }
}
