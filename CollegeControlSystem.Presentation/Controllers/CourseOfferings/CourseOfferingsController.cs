using CollegeControlSystem.Application.CourseOfferings.CancelCourseOffering;
using CollegeControlSystem.Application.CourseOfferings.ChangeInstructor;
using CollegeControlSystem.Application.CourseOfferings.CreateCourseOffering;
using CollegeControlSystem.Application.CourseOfferings.DeleteCourseOffering;
using CollegeControlSystem.Application.CourseOfferings.GetAvailableOfferings;
using CollegeControlSystem.Application.CourseOfferings.GetOfferingAnalytics;
using CollegeControlSystem.Application.CourseOfferings.GetOfferingById;
using CollegeControlSystem.Application.CourseOfferings.GetRoster;
using CollegeControlSystem.Application.CourseOfferings.UpdateCourseOffering;
using CollegeControlSystem.Application.CourseOfferings.UpdateOfferingCapacity;
using CollegeControlSystem.Application.Registrations.ExportGrades;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Domain.Registrations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollegeControlSystem.Presentation.Controllers.CourseOfferings
{
    [Route("api/course-offerings")]
    [ApiController]
    public sealed class CourseOfferingsController : ControllerBase
    {
        private readonly ISender _sender;

        public CourseOfferingsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a new course offering for a specific semester. Requires Admin role.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> CreateCourseOffering(
            [FromBody] CreateCourseOfferingRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateCourseOfferingCommand(
                request.CourseId,
                request.InstructorId,
                request.Term,
                request.Year,
                request.Capacity);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            // Returns 201 Created
            // Note: Since we don't have a "GetById" endpoint yet, we just return the ID in the body/location
            return CreatedAtAction(nameof(GetAvailableOfferings), new { term = request.Term, year = request.Year }, result.Value);
        }

        /// <summary>
        /// Retrieves a paginated list of course offerings with optional filters (term, year, course, instructor). Requires authentication.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAvailableOfferings(
            [FromQuery] string? term,
            [FromQuery] int? year,
            [FromQuery] Guid? courseId,
            [FromQuery] Guid? instructorId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAvailableOfferingsQuery(term, year, courseId, instructorId, page, pageSize);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Updates the capacity of a course offering. Requires Admin role.
        /// </summary>
        [HttpPut("{id:guid}/capacity")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> UpdateCapacity(
            Guid id,
            [FromBody] UpdateCapacityRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateOfferingCapacityCommand(id, request.NewCapacity);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseOfferingErrors.OfferingNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Changes the instructor assigned to a course offering. Requires Admin role.
        /// </summary>
        [HttpPut("{id:guid}/instructor")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> ChangeInstructor(
            Guid id,
            [FromBody] ChangeInstructorRequest request,
            CancellationToken cancellationToken)
        {
            var command = new ChangeInstructorCommand(id, request.NewInstructorId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseOfferingErrors.OfferingNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }
        /// <summary>
        /// Gets the student roster for a course offering. Requires Professor or Admin role.
        /// </summary>
        [HttpGet("{id:guid}/roster")]
        [Authorize(Roles = Roles.ProfessorRole + "," + Roles.AdminRole)]
        public async Task<IActionResult> GetCourseRoster(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetCourseRosterQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure) return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Exports the course roster and grades as a CSV file.
        /// </summary>
        [HttpGet("{id:guid}/export-grades")]
        [Authorize(Roles = Roles.ProfessorRole + "," + Roles.AdminRole)]
        public async Task<IActionResult> ExportGrades(Guid id, CancellationToken cancellationToken)
        {
            // 1. Extract the User ID from the JWT Token for security validation
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out Guid requestingUserId))
            {
                return Unauthorized(new { Error = "Invalid user token." });
            }

            // 2. Check if the user is an Admin (Admins can export any course)
            bool isAdmin = User.IsInRole(Roles.AdminRole);

            // 3. Send Query
            var query = new ExportGradesQuery(id, requestingUserId, isAdmin);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == GradeErrors.Unauthorized)
                {
                    return StatusCode(403, result.Error); // 403 Forbidden
                }

                return BadRequest(result.Error);
            }

            // 4. Return the File Result
            // ASP.NET Core will automatically set the Content-Disposition header to trigger a file download in the browser
            return File(
                fileContents: result.Value.Content,
                contentType: result.Value.ContentType,
                fileDownloadName: result.Value.FileName);
        }

        /// <summary>
        /// Returns enrollment and grade analytics for a course offering. Requires Admin or Professor role.
        /// </summary>
        [HttpGet("{id:guid}/analytics")]
        [Authorize(Roles = Roles.AdminRole + "," + Roles.ProfessorRole)]
        public async Task<IActionResult> GetOfferingAnalytics(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetOfferingAnalyticsQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseOfferingErrors.OfferingNotFound)
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Updates a course offering (capacity and instructor). Requires Admin role.
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> UpdateOffering(
            Guid id,
            [FromBody] UpdateCourseOfferingRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateCourseOfferingCommand(id, request.NewCapacity, request.NewInstructorId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseOfferingErrors.OfferingNotFound)
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Gets course offering details by ID. Requires authentication.
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetOfferingByIdQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseOfferingErrors.OfferingNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Deletes a course offering. Requires Admin role.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> DeleteOffering(
            Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteCourseOfferingCommand(id);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseOfferingErrors.OfferingNotFound)
                {
                    return NotFound(result.Error);
                }

                if (result.Error == CourseOfferingErrors.HasRegistrations)
                {
                    return Conflict(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Cancels a course offering. Requires Admin role.
        /// </summary>
        [HttpPut("{id:guid}/cancel")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> CancelOffering(
            Guid id,
            CancellationToken cancellationToken)
        {
            var command = new CancelCourseOfferingCommand(id);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseOfferingErrors.OfferingNotFound)
                {
                    return NotFound(result.Error);
                }

                if (result.Error == CourseOfferingErrors.HasEnrolledStudents ||
                    result.Error == CourseOfferingErrors.AlreadyCancelled)
                {
                    return Conflict(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}