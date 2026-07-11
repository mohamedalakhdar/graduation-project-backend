using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;
using CollegeControlSystem.Domain.Departments;
using Microsoft.Extensions.DependencyInjection;

namespace CollegeControlSystem.Infrastructure.Seeders;

public static class AcademicSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        var existing = await unitOfWork.DepartmentRepository.GetByNameAsync("Civil Engineering");
        if (existing != null)
            return;

        var departments = await SeedDepartmentsAsync(unitOfWork);
        await SeedProgramsAsync(unitOfWork, departments);
        await SeedCoursesAsync(unitOfWork, departments);
    }

    private static async Task<List<Department>> SeedDepartmentsAsync(IUnitOfWork unitOfWork)
    {
        var departments = new List<Department>();

        var names = new[]
        {
            ("Architectural Engineering", "ARC - Design, construction, and aesthetics of buildings"),
            ("Civil Engineering", "CIV - Infrastructure and structural engineering"),
            ("Electrical Engineering", "ELE - Power systems and electronics"),
            ("Mechanical Engineering", "MEC - Machinery and thermal systems"),
            ("Engineering Physics and Mathematics", "PHM - Foundational sciences (no degree)")
        };

        foreach (var (name, desc) in names)
        {
            var result = Department.Create(name, desc);
            if (result.IsSuccess)
            {
                unitOfWork.DepartmentRepository.Add(result.Value);
                departments.Add(result.Value);
            }
        }

        await unitOfWork.SaveChangesAsync();
        return departments;
    }

    private static async Task SeedProgramsAsync(IUnitOfWork unitOfWork, List<Department> departments)
    {
        var deptMap = departments.ToDictionary(d => d.DepartmentName);

        var programs = new (string Name, string DeptName, int Credits)[]
        {
            ("Architecture Engineering", "Architectural Engineering", 160),
            ("Civil Engineering", "Civil Engineering", 160),
            ("Computers and Automatic Control Engineering", "Electrical Engineering", 160),
            ("Electrical Power and Machines Engineering", "Electrical Engineering", 160),
            ("Electronic and Electrical Communication Engineering", "Electrical Engineering", 160),
            ("Mechanical Power Engineering", "Mechanical Engineering", 160),
            ("Mechanical Design and Production Engineering", "Mechanical Engineering", 160),
            ("Intelligence Systems Engineering", "Electrical Engineering", 160),
            ("Construction and Building Engineering", "Civil Engineering", 160),
            ("Mechatronics Systems Engineering", "Mechanical Engineering", 160)
        };

        foreach (var (name, deptName, credits) in programs)
        {
            if (deptMap.TryGetValue(deptName, out var dept))
            {
                var programResult = Program.Create(name, credits, dept.Id);
                if (programResult.IsSuccess)
                {
                    await unitOfWork.DepartmentRepository.AddProgramAsync(programResult.Value);
                }
            }
        }

        await unitOfWork.SaveChangesAsync();
    }

    private static async Task SeedCoursesAsync(IUnitOfWork unitOfWork, List<Department> departments)
    {
        var deptMap = departments.ToDictionary(d => d.DepartmentName);

        // Level 0 - Preparatory Courses (All Departments get these)
        var prepCourses = new (string Code, string Title, int Credits, int Lec, int Lab)[]
        {
            ("EM1 101", "Engineering Mathematics 1", 4, 3, 2),
            ("EM2 102", "Engineering Mathematics 2", 4, 3, 2),
            ("EP1 101", "Engineering Physics 1", 3, 2, 2),
            ("EP2 102", "Engineering Physics 2", 3, 2, 2),
            ("EM 101", "Engineering Mechanics 1", 3, 2, 2),
            ("EM 102", "Engineering Mechanics 2", 3, 2, 2),
            ("ED 101", "Engineering Drawing 1", 2, 1, 3),
            ("ED 102", "Engineering Drawing 2", 2, 1, 3),
            ("ECH 101", "Engineering Chemistry", 3, 2, 2),
            ("CP1 101", "Computer Programming 1", 3, 2, 2),
            ("PME 101", "Principles of Manufacturing", 2, 2, 0),
            ("TEL 101", "Technical English", 2, 2, 0),
            ("HIT 101", "History of Engineering", 2, 2, 0),
            ("SHR 101", "Societal Issues", 2, 2, 0)
        };

        foreach (var dept in departments)
        {
            foreach (var (code, title, credits, lec, lab) in prepCourses)
            {
                await AddCourseAsync(unitOfWork, dept.Id, code, title, credits, lec, lab);
            }
        }

        // Civil & Construction Engineering
        if (deptMap.TryGetValue("Civil Engineering", out var civDept))
        {
            var civCourses = new (string Code, string Title, int Credits, int Lec, int Lab)[]
            {
                ("CIV 201", "Theory of Structures", 4, 3, 2),
                ("CIV 202", "Surveying", 3, 2, 2),
                ("CIV 203", "Construction Materials", 3, 2, 2),
                ("CIV 204", "Fluid Mechanics", 4, 3, 2),
                ("CIV 301", "Reinforced Concrete", 4, 3, 2),
                ("CIV 302", "Engineering Geology", 3, 2, 2),
                ("CIV 303", "Traffic Engineering", 3, 2, 2)
            };
            foreach (var (code, title, credits, lec, lab) in civCourses)
                await AddCourseAsync(unitOfWork, civDept.Id, code, title, credits, lec, lab);
        }

        // Architectural Engineering
        if (deptMap.TryGetValue("Architectural Engineering", out var arcDept))
        {
            var arcCourses = new (string Code, string Title, int Credits, int Lec, int Lab)[]
            {
                ("ARC 201", "Architectural Design 1", 4, 3, 2),
                ("ARC 202", "Architectural Construction", 3, 2, 2),
                ("ARC 203", "Visual Studies", 3, 2, 2),
                ("ARC 204", "History of Architecture", 3, 2, 0),
                ("ARC 301", "Urban Planning", 3, 2, 2),
                ("ARC 302", "Working Details", 3, 2, 2),
                ("CIV 201", "Theory of Structures", 4, 3, 2),
                ("CIV 204", "Fluid Mechanics", 4, 3, 2)
            };
            foreach (var (code, title, credits, lec, lab) in arcCourses)
                await AddCourseAsync(unitOfWork, arcDept.Id, code, title, credits, lec, lab);
        }

        // Electrical Engineering (CCE / EPM / ECE)
        if (deptMap.TryGetValue("Electrical Engineering", out var eleDept))
        {
            var eleCourses = new (string Code, string Title, int Credits, int Lec, int Lab)[]
            {
                ("ELE 201", "Electrical Circuits", 4, 3, 2),
                ("ELE 202", "Electronic Circuits", 4, 3, 2),
                ("ELE 203", "Logic Circuits", 4, 3, 2),
                ("ELE 204", "Electromagnetic Fields", 3, 2, 2),
                ("ELE 301", "Microprocessors", 4, 3, 2),
                ("ELE 302", "Signals Analysis", 3, 2, 2),
                ("ELE 303", "Electrical Machines", 4, 3, 2),
                ("ELE 304", "Communication Systems", 3, 2, 2)
            };
            foreach (var (code, title, credits, lec, lab) in eleCourses)
                await AddCourseAsync(unitOfWork, eleDept.Id, code, title, credits, lec, lab);

            // Intelligence & Mechatronics
            var intCourses = new (string Code, string Title, int Credits, int Lec, int Lab)[]
            {
                ("CSE 201", "Object-Oriented Programming", 4, 3, 2),
                ("CSE 202", "Data Structures", 4, 3, 2),
                ("CSE 301", "Artificial Intelligence", 4, 3, 2),
                ("CSE 302", "Machine Learning", 4, 3, 2),
                ("CSE 303", "Robotics Systems", 4, 3, 2),
                ("CSE 304", "Embedded Systems", 4, 3, 2),
                ("CSE 305", "PLC Systems", 3, 2, 2)
            };
            foreach (var (code, title, credits, lec, lab) in intCourses)
                await AddCourseAsync(unitOfWork, eleDept.Id, code, title, credits, lec, lab);
        }

        // Mechanical Engineering
        if (deptMap.TryGetValue("Mechanical Engineering", out var mecDept))
        {
            var mecCourses = new (string Code, string Title, int Credits, int Lec, int Lab)[]
            {
                ("MEC 201", "Thermodynamics", 4, 3, 2),
                ("MEC 202", "Fluid Mechanics", 4, 3, 2),
                ("MEC 203", "Machine Design", 4, 3, 2),
                ("MEC 204", "Mechanics of Materials", 3, 2, 2),
                ("MEC 301", "Heat Transfer", 4, 3, 2),
                ("MEC 302", "Hydraulic Machines", 3, 2, 2),
                ("MEC 303", "Metal Cutting", 3, 2, 2),
                ("MEC 304", "CNC Machines", 4, 3, 2)
            };
            foreach (var (code, title, credits, lec, lab) in mecCourses)
                await AddCourseAsync(unitOfWork, mecDept.Id, code, title, credits, lec, lab);

            // Mechatronics
            var mseCourses = new (string Code, string Title, int Credits, int Lec, int Lab)[]
            {
                ("CSE 201", "Object-Oriented Programming", 4, 3, 2),
                ("CSE 202", "Data Structures", 4, 3, 2),
                ("CSE 303", "Robotics Systems", 4, 3, 2),
                ("CSE 304", "Embedded Systems", 4, 3, 2),
                ("CSE 305", "PLC Systems", 3, 2, 2),
                ("MEC 202", "Fluid Mechanics", 4, 3, 2),
                ("MEC 304", "CNC Machines", 4, 3, 2)
            };
            foreach (var (code, title, credits, lec, lab) in mseCourses)
                await AddCourseAsync(unitOfWork, mecDept.Id, code, title, credits, lec, lab);
        }

        // Engineering Physics and Mathematics
        if (deptMap.TryGetValue("Engineering Physics and Mathematics", out var phmDept))
        {
            var phmCourses = new (string Code, string Title, int Credits, int Lec, int Lab)[]
            {
                ("PHM 201", "Advanced Mathematics", 4, 3, 2),
                ("PHM 202", "Numerical Methods", 3, 2, 2),
                ("PHM 203", "Applied Physics", 3, 2, 2)
            };
            foreach (var (code, title, credits, lec, lab) in phmCourses)
                await AddCourseAsync(unitOfWork, phmDept.Id, code, title, credits, lec, lab);
        }

        await unitOfWork.SaveChangesAsync();
    }

    private static async Task AddCourseAsync(IUnitOfWork unitOfWork, Guid deptId, string code, string title, int credits, int lec, int lab)
    {
        var courseResult = Course.Create(deptId, code, title, $"Core course for {title}", credits, lec, lab);
        if (courseResult.IsSuccess)
        {
            await unitOfWork.CourseRepository.AddAsync(courseResult.Value);
        }
    }
}