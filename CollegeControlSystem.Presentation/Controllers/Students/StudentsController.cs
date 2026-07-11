using CollegeControlSystem.Application.Students.AssignAdvisor;
using CollegeControlSystem.Application.Students.ChangeProgram;
using CollegeControlSystem.Application.Students.CreateStudent;
using CollegeControlSystem.Application.Students.DismissStudent;
using CollegeControlSystem.Application.Students.GetAllStudents;
using CollegeControlSystem.Application.Students.GetStudentProfile;
using CollegeControlSystem.Application.Students.GetTranscript;
using CollegeControlSystem.Application.Students.UpdateStudentProfile;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Domain.Students;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.Students
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly ISender _sender;

        public StudentsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a new student. Requires Admin role.
        /// </summary>
        [HttpPost]
        [Authorize(Roles=Roles.AdminRole)]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetStudentProfile), new { id = result.Value }, result.Value);
        }

        /// <summary>
        /// Gets all students with optional filters and pagination. Requires Admin role.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> GetAllStudents(
            [FromQuery] string? search,
            [FromQuery] Guid? programId,
            [FromQuery] Guid? advisorId,
            [FromQuery] AcademicStatus? status,
            [FromQuery] decimal? minCGPA,
            [FromQuery] decimal? maxCGPA,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllStudentsQuery(
                search, programId, advisorId, status, minCGPA, maxCGPA, page, pageSize);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets the authenticated student's own profile from JWT.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            var query = new GetMyStudentProfileQuery(User);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets student profile information. Accessible to all users.
        /// </summary>
        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetStudentProfile(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetStudentProfileQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets a student's academic transcript with semester-by-semester grades and SGPA. Accessible to all users.
        /// </summary>
        [HttpGet("{id}/transcript")]
        public async Task<IActionResult> GetTranscript(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetTranscriptQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Updates student profile details. Accessible to all users.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateStudentProfileRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateStudentProfileCommand(id, request.NewFullName, request.NewNationalId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Assigns an advisor to a student. Accessible to all users.
        /// </summary>
        [HttpPut("{id}/assign-advisor")]
        public async Task<IActionResult> AssignAdvisor(Guid id, [FromBody] AssignAdvisorRequest request, CancellationToken cancellationToken)
        {
            var command = new AssignAdvisorCommand(id, request.AdvisorId);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Dismisses a student. Requires Admin role.
        /// </summary>
        [HttpPut("{id}/dismiss")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> DismissStudent(Guid id, CancellationToken cancellationToken)
        {
            var command = new DismissStudentCommand(id);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Changes a student's program. Requires Admin role.
        /// </summary>
        [HttpPut("{id}/program")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> ChangeProgram(Guid id, [FromBody] ChangeProgramRequest request, CancellationToken cancellationToken)
        {
            var command = new ChangeProgramCommand(id, request.NewProgramId);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }


}
