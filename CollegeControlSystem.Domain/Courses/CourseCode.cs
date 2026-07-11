using CollegeControlSystem.Domain.Abstractions;
using System.Text.RegularExpressions;

namespace CollegeControlSystem.Domain.Courses
{
    public sealed record CourseCode
    {
        // Example: CCE 123 – three letters + three digits
        private static readonly Regex CodePattern =
            new(@"^([A-Z]{3})\s?([0-9]{3})$", RegexOptions.Compiled);

        public string Value { get; }
        public string DepartmentPart { get; }
        public int LevelPart { get; }

        private CourseCode(string value, string department, int level)
        {
            Value = value;
            DepartmentPart = department;
            LevelPart = level;
        }

        public static Result<CourseCode> Create(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Result<CourseCode>.Failure(CourseErrors.CodeEmpty);

            var normalized = code.Trim().ToUpper();

            var match = CodePattern.Match(normalized);
            if (!match.Success)
                return Result<CourseCode>.Failure(CourseErrors.CodeInvalidFormat);

            var dept = match.Groups[1].Value;      // CCE
            var numericPart = match.Groups[2].Value; // 123

            // first digit represents course level (1 = 100-level)
            var level = int.Parse(numericPart[0].ToString());

            // canonical final format: "CCE 123"
            var finalValue = $"{dept} {numericPart}";

            return Result<CourseCode>.Success(
                new CourseCode(finalValue, dept, level));
        }

        public override string ToString() => Value; // so u can do Course.Code or Course.code.toString() it will retucrn the Value
    }
}

