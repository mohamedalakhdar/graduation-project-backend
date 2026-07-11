using CollegeControlSystem.Application.Courses.AddPrerequisite;
using CollegeControlSystem.Application.Courses.CreateCourse;
using CollegeControlSystem.Application.Courses.DeleteCourse;
using CollegeControlSystem.Application.Courses.GetCourseDetails;
using CollegeControlSystem.Application.Courses.GetCourseList;
using CollegeControlSystem.Application.Courses.GetPrerequisites;
using CollegeControlSystem.Application.Courses.RemovePrerequisite;
using CollegeControlSystem.Application.Courses.UpdateCourse;
using CollegeControlSystem.Domain.Courses;
using CollegeControlSystem.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.Courses
{
    [Route("api/courses")]
    [ApiController]
    public sealed class CoursesController : ControllerBase
    {
        private readonly ISender _sender;

        public CoursesController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a new course. Requires Admin role.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> CreateCourse(
            [FromBody] CreateCourseRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateCourseCommand(
                request.DepartmentId,
                request.Code,
                request.Title,
                request.Description,
                request.Credits,
                request.LectureHours,
                request.LabHours);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                // Handle duplicate code specifically if needed
                if (result.Error == CourseErrors.DuplicateCode)
                {
                    return Conflict(result.Error); // HTTP 409 Conflict
                }

                return BadRequest(result.Error);
            }

            // Return 201 Created with a Location header pointing to the GetById endpoint
            return CreatedAtAction(nameof(GetCourseDetails), new { id = result.Value }, result.Value);
        }

        /// <summary>
        /// Retrieves a paginated list of courses, optionally filtered by search, department, or credits. Requires authentication.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCourseList(
            [FromQuery] string? search,
            [FromQuery] Guid? departmentId,
            [FromQuery] int? minCredits,
            [FromQuery] int? maxCredits,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetCourseListQuery(search, departmentId, minCredits, maxCredits, page, pageSize);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets course details by ID. Requires authentication.
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetCourseDetails(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetCourseDetailsQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseErrors.CourseNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        // --- Prerequisite Sub-Resources ---

        /// <summary>
        /// Adds a prerequisite to a course. Requires Admin role.
        /// </summary>
        [HttpPost("{id:guid}/prerequisites")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> AddPrerequisite(
            Guid id,
            [FromBody] AddPrerequisiteRequest request,
            CancellationToken cancellationToken)
        {
            // 'id' from route is the Course, 'request.PrerequisiteCourseId' is the dependency
            var command = new AddPrerequisiteCommand(id, request.PrerequisiteCourseId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseErrors.CourseNotFound ||
                    result.Error == CourseErrors.PrerequisiteNotFound)
                {
                    return NotFound(result.Error);
                }

                // e.g. CircularDependency error
                return BadRequest(result.Error);
            }

            return NoContent(); // 204 No Content is standard for "Action completed, nothing to return"
        }

        /// <summary>
        /// Removes a prerequisite from a course. Requires Admin role.
        /// </summary>
        [HttpDelete("{id:guid}/prerequisites/{prerequisiteId:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> RemovePrerequisite(
            Guid id,
            Guid prerequisiteId,
            CancellationToken cancellationToken)
        {
            var command = new RemovePrerequisiteCommand(id, prerequisiteId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseErrors.CourseNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Updates course information. Requires Admin role.
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> UpdateCourse(
            Guid id,
            [FromBody] UpdateCourseRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateCourseCommand(
                id,
                request.DepartmentId,
                request.Title,
                request.Description,
                request.Credits,
                request.LectureHours,
                request.LabHours);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseErrors.CourseNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a course. Requires Admin role.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> DeleteCourse(
            Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteCourseCommand(id);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseErrors.CourseNotFound)
                {
                    return NotFound(result.Error);
                }

                if (result.Error == CourseErrors.HasOfferings || result.Error == CourseErrors.HasRegistrations)
                {
                    return Conflict(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Gets prerequisites for a course. Requires authentication.
        /// </summary>
        [HttpGet("{id:guid}/prerequisites")]
        [Authorize]
        public async Task<IActionResult> GetPrerequisites(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetPrerequisitesQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == CourseErrors.CourseNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}