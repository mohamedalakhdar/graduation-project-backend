using CollegeControlSystem.Application.Registrations.AppealGrade;
using CollegeControlSystem.Application.Registrations.ApproveRegistration;
using CollegeControlSystem.Application.Registrations.DropCourse;
using CollegeControlSystem.Application.Registrations.GetAllRegistrations;
using CollegeControlSystem.Application.Registrations.GetAvailableCourses;
using CollegeControlSystem.Application.Registrations.GetGrade;
using CollegeControlSystem.Application.Registrations.GetPendingRegistrations;
using CollegeControlSystem.Application.Registrations.GetRegistrationById;
using CollegeControlSystem.Application.Registrations.GetStudentRegistrations;
using CollegeControlSystem.Application.Registrations.GetStudentSchedule;
using CollegeControlSystem.Application.Registrations.RegisterCourse;
using CollegeControlSystem.Application.Registrations.ReviewAppeal;
using CollegeControlSystem.Application.Registrations.SubmitGrades;
using CollegeControlSystem.Application.Registrations.WithdrawCourse;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;  
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollegeControlSystem.Presentation.Controllers.Registratoins
{
    [Route("api/registrations")]
    [ApiController]
    public sealed class RegistrationsController : ControllerBase
    {
        private readonly ISender _sender;

        public RegistrationsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Gets all registrations globally with optional filters and pagination. Requires Admin role.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> GetAllRegistrations(
            [FromQuery] RegistrationStatus? status,
            [FromQuery] Guid? studentId,
            [FromQuery] Guid? courseOfferingId,
            [FromQuery] string? semester,
            [FromQuery] int? year,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllRegistrationsQuery(
                status, studentId, courseOfferingId, semester, year, page, pageSize);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Registers a student for a course offering. Requires Student role.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.StudentRole)]
        public async Task<IActionResult> RegisterCourse(
            [FromBody] RegisterCourseRequest request,
            CancellationToken cancellationToken)
        {
            var command = new RegisterCourseCommand(
                request.StudentId,
                request.CourseOfferingId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                // Map Domain Errors to specific HTTP Status Codes for better client handling
                if (result.Error == RegistrationErrors.DuplicateRegistration)
                {
                    return Conflict(result.Error); // 409 Conflict
                }

                if (result.Error == StudentErrors.StudentNotFound ||
                    result.Error == CourseOfferingErrors.OfferingNotFound)
                {
                    return NotFound(result.Error); // 404 Not Found
                }

                // Default for Prerequisite failure, Overload, Capacity full, etc.
                return BadRequest(result.Error); // 400 Bad Request
            }

            // Return 201 Created
            // We use the GetStudentSchedule endpoint as the "Location" for the new resource context
            return CreatedAtAction(
                nameof(GetStudentSchedule),
                new { studentId = request.StudentId },
                new { registrationId = result.Value });
        }

        /// <summary>
        /// Approves a pending registration. Requires Advisor or Admin role.
        /// </summary>
        [HttpPut("{id:guid}/approve")]
        [Authorize(Roles = Roles.AdvisorRole + "," + Roles.AdminRole)]
        public async Task<IActionResult> ApproveRegistration(
            Guid id,
            [FromBody] ApproveRegistrationRequest request,
            CancellationToken cancellationToken)
        {
            // 'id' is the RegistrationId from the URL
            var command = new ApproveRegistrationCommand(id, request.AdvisorId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == RegistrationErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                if (result.Error == RegistrationErrors.Unauthorized)
                {
                    return StatusCode(403, result.Error); // 403 Forbidden
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Drops a registered course. Requires Student role.
        /// </summary>
        [HttpPut("{id:guid}/drop")]
        [Authorize(Roles = Roles.StudentRole)]
        public async Task<IActionResult> DropCourse(
            Guid id,
            [FromBody] DropCourseRequest request,
            CancellationToken cancellationToken)
        {
            var command = new DropCourseCommand(id, request.StudentId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == RegistrationErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                if (result.Error == RegistrationErrors.Unauthorized)
                {
                    return StatusCode(403, result.Error);
                }

                // e.g., Cannot drop a course that is already graded/completed
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Withdraws from a course after the drop period. Requires Student role.
        /// </summary>
        [HttpPut("{id:guid}/withdraw")]
        [Authorize(Roles = Roles.StudentRole)]
        public async Task<IActionResult> WithdrawCourse(
            Guid id,
            [FromBody] WithdrawCourseRequest request,
            CancellationToken cancellationToken)
        {
            var command = new WithdrawCourseCommand(id, request.StudentId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == RegistrationErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                if (result.Error == RegistrationErrors.Unauthorized)
                {
                    return StatusCode(403, result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Gets pending registrations for an advisor. Requires Advisor or Admin role.
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = Roles.AdvisorRole + "," + Roles.AdminRole)]
        public async Task<IActionResult> GetPendingRegistrations(
            [FromQuery] Guid advisorId,
            CancellationToken cancellationToken)
        {
            var query = new GetPendingRegistrationsQuery(advisorId);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets a student's current schedule. Requires Student role.
        /// </summary>
        [HttpGet("schedule/{studentId:guid}")]
        [Authorize(Roles = Roles.StudentRole)]
        public async Task<IActionResult> GetStudentSchedule(
            Guid studentId,
            [FromQuery] string? term,
            [FromQuery] int? year,
            CancellationToken cancellationToken)
        {
            var query = new GetStudentScheduleQuery(studentId, term, year);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == StudentErrors.StudentNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Shows students the available courses they can register for in the specified semester.
        /// </summary>
        [HttpGet("available")]
        [Authorize(Roles = Roles.StudentRole)] // Only Students access their registration board
        public async Task<IActionResult> GetAvailableCoursesForRegistration(
            [FromQuery] Guid studentId,
            [FromQuery] string term,
            [FromQuery] int year,
            CancellationToken cancellationToken)
        {
            var query = new GetAvailableCoursesQuery(studentId, term, year);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == StudentErrors.StudentNotFound)
                {
                    return NotFound(result.Error);
                }

                // E.g., Invalid Semester Term provided
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets all registrations for a student. Requires Student, Advisor, or Admin role.
        /// </summary>
        [HttpGet("student/{studentId:guid}")]
        [Authorize(Roles = Roles.StudentRole + "," + Roles.AdvisorRole + "," + Roles.AdminRole)]
        public async Task<IActionResult> GetStudentRegistrations(
            Guid studentId,
            CancellationToken cancellationToken)
        {
            var query = new GetStudentRegistrationsQuery(studentId);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == StudentErrors.StudentNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets registration details by ID. Requires authentication.
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetRegistrationByIdQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == RegistrationErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Submits semester work and final exam grades for students.
        /// Automatically calculates total points, letter grades, and applies retake caps.
        /// </summary>
        [HttpPost("submit")]
        [Authorize(Roles = Roles.ProfessorRole + "," + Roles.AdminRole)] // Instructors submit grades
        public async Task<IActionResult> SubmitGrades(
            [FromBody] SubmitGradesRequest request,
            CancellationToken cancellationToken)
        {
            // Security Best Practice: Extract the Instructor ID from the JWT Claims
            // ClaimTypes.NameIdentifier usually stores the User ID when generating the token
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid instructorId))
            {
                return Unauthorized(new { Error = "Invalid user token." });
            }

            var command = new SubmitGradesCommand(
                request.OfferingId,
                instructorId,
                request.Submissions);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                // E.g., Returns 400 if scores exceed 100 or are negative
                return BadRequest(result.Error);
            }

            return Ok(new { Message = "Grades successfully submitted and applied." });
        }

        /// <summary>
        /// Gets the grade for a registration. Requires Student, Advisor, or Admin role.
        /// </summary>
        [HttpGet("{id:guid}/grade")]
        [Authorize(Roles = Roles.StudentRole + "," + Roles.AdvisorRole + "," + Roles.AdminRole)]
        public async Task<IActionResult> GetGrade(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetGradeQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == RegistrationErrors.NotFound || result.Error == GradeErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Appeals a grade. Requires Student role.
        /// </summary>
        [HttpPost("{id:guid}/grade/appeal")]
        [Authorize(Roles = Roles.StudentRole)]
        public async Task<IActionResult> AppealGrade(Guid id, [FromBody] AppealGradeRequest request, CancellationToken cancellationToken)
        {
            var studentIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(studentIdClaim, out Guid studentId))
            {
                return Unauthorized(new { Error = "Invalid user token." });
            }

            var command = new AppealGradeCommand(id, request.Reason);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == RegistrationErrors.NotFound || result.Error == GradeAppealErrors.NoGradeToAppeal)
                {
                    return NotFound(result.Error);
                }

                if (result.Error == GradeAppealErrors.AlreadyAppealed)
                {
                    return Conflict(result.Error);
                }

                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetGrade), new { id = result.Value }, new { appealId = result.Value });
        }

        /// <summary>
        /// Reviews a grade appeal. Requires Admin or Advisor role.
        /// </summary>
        [HttpPut("appeals/{id:guid}/review")]
        [Authorize(Roles = Roles.AdminRole + "," + Roles.AdvisorRole)]
        public async Task<IActionResult> ReviewAppeal(Guid id, [FromBody] ReviewAppealRequest request, CancellationToken cancellationToken)
        {
            var reviewerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(reviewerIdClaim, out Guid reviewerId))
            {
                return Unauthorized(new { Error = "Invalid user token." });
            }

            if (!Enum.TryParse<GradeAppealStatus>(request.Status, true, out GradeAppealStatus status))
            {
                return BadRequest(new { Error = "Invalid appeal status. Must be 'Approved' or 'Rejected'." });
            }

            var command = new ReviewAppealCommand(id, status, request.ReviewNotes, reviewerId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == GradeAppealErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                if (result.Error == GradeAppealErrors.AlreadyReviewed)
                {
                    return Conflict(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}