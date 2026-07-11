using CollegeControlSystem.Application.Faculties.ChangeFacultyStatus;
using CollegeControlSystem.Application.Faculties.CreateFaculty;
using CollegeControlSystem.Application.Faculties.GetAdvisorStudents;
using CollegeControlSystem.Application.Faculties.GetFacultyById;
using CollegeControlSystem.Application.Faculties.GetAdvisorsList;
using CollegeControlSystem.Application.Faculties.GetFacultyList;
using CollegeControlSystem.Application.Faculties.GetInstructorCourses;
using CollegeControlSystem.Application.Faculties.TransferDepartment;
using CollegeControlSystem.Application.Faculties.UpdateFacultyInfo;
using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.Faculties
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacultyController : ControllerBase
    {
        private readonly ISender _sender;

        public FacultyController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a new faculty member. (Admin usage)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetFacultyById), new { id = result.Value }, result.Value);
        }

        /// <summary>
        /// Retrieves a paginated list of faculty members, optionally filtered by search, department, or status.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFacultyList(
            [FromQuery] string? search,
            [FromQuery] Guid? departmentId,
            [FromQuery] FacultyStatus? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetFacultyListQuery(search, departmentId, status, page, pageSize);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieves the profile of a specific faculty member.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetFacultyById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetFacultyByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Updates a faculty member's degree (e.g., "PhD", "Professor").
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> UpdateInfo(Guid id, [FromBody] UpdateFacultyInfoRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateFacultyInfoCommand(id, request.NewDegree);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Transfers a faculty member to a different department. (Admin usage)
        /// </summary>
        [HttpPut("{id}/transfer")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> TransferDepartment(Guid id, [FromBody] TransferDepartmentRequest request, CancellationToken cancellationToken)
        {
            var command = new TransferDepartmentCommand(id, request.NewDepartmentId);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Changes a faculty member's status (Active, Resigned, Retired, Dismissed).
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeFacultyStatusRequest request, CancellationToken cancellationToken)
        {
            var command = new ChangeFacultyStatusCommand(id, request.NewStatus);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Retrieves a paginated list of faculty members who have the Advisor role.
        /// </summary>
        [HttpGet("advisors")]
        [Authorize]
        public async Task<IActionResult> GetAdvisorsList(
            [FromQuery] string? search,
            [FromQuery] Guid? departmentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAdvisorsListQuery(search, departmentId, page, pageSize);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        // --- INSTRUCTOR ROLE ENDPOINTS ---

        /// <summary>
        /// Gets courses taught by an instructor. Requires Professor or Admin role.
        /// </summary>
        [HttpGet("{id}/courses")]
        [Authorize(Roles = Roles.ProfessorRole + "," + Roles.AdminRole)]
        public async Task<IActionResult> GetInstructorCourses(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetInstructorCoursesQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        // --- ADVISOR ROLE ENDPOINTS ---

        /// <summary>
        /// Gets students advised by an advisor. Requires Advisor or Admin role.
        /// </summary>
        [HttpGet("{id}/advisees")]
        [Authorize(Roles = Roles.AdvisorRole + "," + Roles.AdminRole)]
        public async Task<IActionResult> GetAdvisorStudents(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetAdvisorStudentsQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }

    public record ChangeFacultyStatusRequest(FacultyStatus NewStatus);
}
