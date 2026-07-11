using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Departments;
using CollegeControlSystem.Domain.Identity;
namespace CollegeControlSystem.Domain.Faculties
{
    public class Faculty:Entity
    {
        // Private constructor for EF Core hydration
        private Faculty() { }

        private Faculty(
            Guid id,
            string facultyName,
            Guid departmentId,
            string appUserId,
            FacultyDegree degree):base(id)
  
        {
            DepartmentId = departmentId;
            Degree = degree;
            FacultyName = facultyName;
            AppUserId = appUserId;
            Status = FacultyStatus.Active;
        }

        public string FacultyName { get; private set; }
        public Guid DepartmentId { get; private set; } // FK to Departments
        public Department Department { get; private set; } // Navigation property
        public FacultyDegree Degree { get; private set; }     // e.g. "PhD", "Associate Professor"

        public string AppUserId { get; private set; } // FK to Identity User
        public AppUser AppUser { get; private set; } // Navigation property

        public FacultyStatus Status { get; private set; } = FacultyStatus.Active;

        // Factory Method
        public static Result<Faculty> Create(
            string facultyName,
            Guid departmentId,
            string appUserId,
            FacultyDegree degree
            ) 
        {
            // 1. Validation
            if (departmentId == Guid.Empty)
                return Result<Faculty>.Failure(Error.EmptyId("Department"));
            if (String.IsNullOrEmpty(appUserId))
                return Result<Faculty>.Failure(Error.EmptyId("User"));
            if (string.IsNullOrWhiteSpace(facultyName))
                return Result<Faculty>.Failure(FacultyErrors.FacultyNameRequired);
            if (!Enum.IsDefined(typeof(FacultyDegree),degree))
                return Result<Faculty>.Failure(FacultyErrors.DegreeRequired);

            // 2. Create Instance
            return Result<Faculty>.Success(
                new Faculty(
                    Guid.NewGuid(),
                    facultyName,
                    departmentId,
                    appUserId,
                    degree
                )   
            );
        }

        // --- Domain Behaviors ---

        public Result UpdateDegree(FacultyDegree newDegree)
        {
            if (!Enum.IsDefined(typeof(FacultyDegree),newDegree))
                return Result.Failure(FacultyErrors.DegreeRequired);

            Degree = newDegree;
            return Result.Success();
        }

        /// <summary>
        /// Transfers the faculty member to a new department.
        /// This might affect which courses they are allowed to teach.
        /// </summary>
        public Result TransferDepartment(Guid newDepartmentId)
        {
            if( newDepartmentId == Guid.Empty) return Result.Failure(FacultyErrors.DepartmentRequired);

            if (newDepartmentId == DepartmentId)
                return Result.Failure(FacultyErrors.SameDepartment);

            DepartmentId = newDepartmentId;
            return Result.Success();
        }

        public void ChangeStatus(FacultyStatus newStatus)
        {
            Status = newStatus;
        }
    }
}
