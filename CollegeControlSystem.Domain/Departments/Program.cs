using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Departments
{
    public sealed class Program: Entity
    {
        private Program(Guid id,string name, int requiredCredits, Guid departmentId):base(id)
        {
            Name = name;
            RequiredCredits = requiredCredits;
            DepartmentId = departmentId;
        }
        // for EF
        private Program()
        {
        }

        public string Name { get; private set; }
        public int RequiredCredits { get; private set; }
        public Guid DepartmentId { get; private set; }

        public Department Department { get; private set; }

        public static Result<Program> Create(string name, int requiredCredits, Guid departmentId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Program>.Failure(DepartmentErrors.NameRequired);

            if (requiredCredits <= 0)
                return Result<Program>.Failure(DepartmentErrors.InvalidCredits);

            if (departmentId == Guid.Empty)
                return Result<Program>.Failure(DepartmentErrors.DepartmentRequired);

            var program = new Program(
                           Guid.NewGuid(),
                           name,
                           requiredCredits,
                           departmentId);

            return Result<Program>.Success(program);
        }

        public Result UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return Result.Failure(DepartmentErrors.NameRequired);
            Name = newName;
            return Result.Success();
        }

        public Result UpdateCredits(int newCredits)
        {
            if (newCredits <= 0)
                return Result.Failure(DepartmentErrors.InvalidCredits);                

            RequiredCredits = newCredits;
            return Result.Success();
        }
    }
}
