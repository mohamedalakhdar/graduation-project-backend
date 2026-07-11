using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Departments
{
    public sealed class Department:Entity
    {
        private Department(Guid id, string name, string description):base(id)
        {
            DepartmentName = name;
            Description = description;
            //IsActive = true;
        }

        // for EF
        private Department()
        {
        }

        public string DepartmentName { get; private set; }
        public string? Description { get; private set; }
        //public bool IsActive { get; private set; }

        //public List<Program> Programs { get; private set; } = new();
        private readonly List<Program> _programs = new();
        public IReadOnlyCollection<Program> Programs => _programs;

        public static Result<Department> Create( string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Department>.Failure(DepartmentErrors.NameRequired);

            return Result<Department>.Success(new Department(Guid.NewGuid(), name, description));
        }

        public Result<Program> AddProgram(string name, int requiredCredits)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Program>.Failure(DepartmentErrors.NameRequired);

            if (requiredCredits <= 0)
                return Result<Program>.Failure(DepartmentErrors.InvalidCredits);

            // Check for duplicates within this department
            if (Programs.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return Result<Program>.Failure(DepartmentErrors.DuplicateProgram);

            var result =Program.Create(name, requiredCredits, this.Id);
            if (result.IsFailure) return Result<Program>.Failure(result.Error);
            var program = result.Value!;
            _programs.Add(program);

            return Result<Program>.Success(program);
        }

        public Result UpdateProgramCredits(Guid programId, int newCredits)
        {
            if (newCredits <= 0)
                return Result.Failure(DepartmentErrors.InvalidCredits);

            var program = Programs.FirstOrDefault(p => p.Id == programId);
            if (program is null)
            {
                return Result.Failure(DepartmentErrors.ProgramNotFound);
            }

            program.UpdateCredits(newCredits);
            return Result.Success();
        }

        public Result UpdateDetails(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Department>.Failure(DepartmentErrors.NameRequired);
            DepartmentName = name;
            Description = description;
            return Result.Success();
        }

        public Program? GetProgram(Guid programId)
        {
            return Programs.FirstOrDefault(p => p.Id == programId);
        }

        public Result RemoveProgram(Guid programId)
        {
            var program = GetProgram(programId);
            if (program is null)
                return Result.Failure(DepartmentErrors.ProgramNotFound);

            _programs.Remove(program);
            return Result.Success();
        }

        public Result UpdateProgramName(Guid programId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return Result.Failure(DepartmentErrors.NameRequired);

            var program = GetProgram(programId);
            if (program is null)
                return Result.Failure(DepartmentErrors.ProgramNotFound);

            if (Programs.Any(p => p.Id != programId && p.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
                return Result.Failure(DepartmentErrors.DuplicateProgram);

            program.UpdateName(newName);
            return Result.Success();
        }
    }
}
