using CollegeControlSystem.Application.Departments.AddProgram;
using CollegeControlSystem.Application.Departments.CreateDepartment;
using CollegeControlSystem.Application.Departments.DeleteDepartment;
using CollegeControlSystem.Application.Departments.DeleteProgram;
using CollegeControlSystem.Application.Departments.GetDepartmentById;
using CollegeControlSystem.Application.Departments.GetDepartments;
using CollegeControlSystem.Application.Departments.GetProgramById;
using CollegeControlSystem.Application.Departments.GetPrograms;
using CollegeControlSystem.Application.Departments.UpdateDepartment;
using CollegeControlSystem.Application.Departments.UpdateProgram;
using CollegeControlSystem.Application.Departments.UpdateProgramCredits;
using CollegeControlSystem.Domain.Departments;
using CollegeControlSystem.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.Departments
{
    [Route("api/departments")]
    [ApiController]
    public sealed class DepartmentsController : ControllerBase
    {
        private readonly ISender _sender;

        public DepartmentsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a new department. Requires Admin role.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> CreateDepartment(
            [FromBody] CreateDepartmentRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateDepartmentCommand(request.Name, request.Description);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            // Returns 201 Created with the location header (optional) and the ID
            return CreatedAtAction(nameof(GetDepartments), new { id = result.Value }, result.Value);
        }

        /// <summary>
        /// Retrieves a paginated list of departments, optionally filtered by search. Requires authentication.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDepartments(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetDepartmentsQuery(search, page, pageSize);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                // Queries usually don't fail in this way, but good safety net
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets department details by ID. Requires authentication.
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetDepartmentById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetDepartmentByIdQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Updates department information. Requires Admin role.
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> UpdateDepartment(
            Guid id,
            [FromBody] UpdateDepartmentRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateDepartmentCommand(id, request.Name, request.Description);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                // Check if the error is specifically a "Not Found" error to return 404
                if (result.Error == DepartmentErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent(); // Standard 204 response for updates
        }

        /// <summary>
        /// Deletes a department. Requires Admin role.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> DeleteDepartment(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteDepartmentCommand(id);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == DepartmentErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        // --- Program Sub-Resources ---

        /// <summary>
        /// Adds a program to a department. Requires Admin role.
        /// </summary>
        [HttpPost("{departmentId:guid}/programs")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> AddProgram(
            Guid departmentId,
            [FromBody] AddProgramRequest request,
            CancellationToken cancellationToken)
        {
            var command = new AddProgramCommand(
                departmentId,
                request.Name,
                request.RequiredCredits);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == DepartmentErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets a paginated list of programs, optionally filtered by search or department. Requires authentication.
        /// </summary>
        [HttpGet("programs")]
        [Authorize]
        public async Task<IActionResult> GetAllPrograms(
            [FromQuery] string? search,
            [FromQuery] Guid? departmentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetProgramsQuery(search, departmentId, page, pageSize);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        //[HttpPut("{departmentId:guid}/programs/{programId:guid}/credits")]
        /// <summary>
        /// Updates the required credits for a program. Requires Admin role.
        /// </summary>
        [HttpPut("programs/{programId:guid}/credits")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> UpdateProgramCredits(
            //Guid departmentId,
            Guid programId,
            [FromBody] UpdateProgramCreditsRequest request,
            CancellationToken cancellationToken)
        {
            //var command = new UpdateProgramCreditsCommand(
            //    departmentId,
            //    programId,
            //    request.NewRequiredCredits);

            var command = new UpdateProgramCreditsCommand(
                programId,
                request.NewRequiredCredits);


            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == DepartmentErrors.NotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        //[HttpGet("{departmentId:guid}/programs/{programId:guid}")]
        /// <summary>
        /// Gets program details by ID. Requires authentication.
        /// </summary>
        [HttpGet("programs/{programId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetProgramById(
            //Guid departmentId,
            Guid programId,
            CancellationToken cancellationToken)
        {
            //var query = new GetProgramByIdQuery(departmentId, programId);
            var query = new GetProgramByIdQuery( programId);

            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        //[HttpPut("{departmentId:guid}/programs/{programId:guid}")]
        /// <summary>
        /// Updates program information. Requires Admin role.
        /// </summary>
        [HttpPut("programs/{programId:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> UpdateProgram(
            //Guid departmentId,
            Guid programId,
            [FromBody] UpdateProgramRequest request,
            CancellationToken cancellationToken)
        {
            //var command = new UpdateProgramCommand(departmentId, programId, request.Name);
            var command = new UpdateProgramCommand( programId, request.Name);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == DepartmentErrors.NotFound || result.Error == DepartmentErrors.ProgramNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        //[HttpDelete("{departmentId:guid}/programs/{programId:guid}")]
        /// <summary>
        /// Deletes a program. Requires Admin role.
        /// </summary>
        [HttpDelete("programs/{programId:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> DeleteProgram(
            //Guid departmentId,
            Guid programId,
            CancellationToken cancellationToken)
        {
            //var command = new DeleteProgramCommand(departmentId, programId);
            var command = new DeleteProgramCommand( programId);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == DepartmentErrors.NotFound || result.Error == DepartmentErrors.ProgramNotFound)
                {
                    return NotFound(result.Error);
                }

                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}
