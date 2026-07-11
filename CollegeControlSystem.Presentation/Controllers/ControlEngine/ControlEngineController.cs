using CollegeControlSystem.Application.ControlEngine.GetGraduates;
using CollegeControlSystem.Application.ControlEngine.GetStatistics;
using CollegeControlSystem.Application.ControlEngine.GetWarnings;
using CollegeControlSystem.Application.ControlEngine.RunEngine;
using CollegeControlSystem.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.ControlEngine
{
    [Route("api/control")]
    [ApiController]
    public sealed class ControlEngineController : ControllerBase
    {
        private readonly ISender _sender;

        public ControlEngineController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Retrieves a list of all students currently under Academic Warning or who have been Dismissed.
        /// </summary>
        [HttpGet("warnings")]
        [Authorize(Roles = Roles.AdminRole)] // Strictly Admin only
        public async Task<IActionResult> GetAcademicWarnings(CancellationToken cancellationToken)
        {
            var query = new GetAcademicWarningsQuery();
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        /// <summary>
        /// Audits Senior students and returns a list of candidates eligible for graduation, 
        /// along with a list of missing requirements for those who aren't quite ready.
        /// </summary>
        [HttpGet("graduates")]
        [Authorize(Roles = Roles.AdminRole)] // Strictly Admin only
        public async Task<IActionResult> GetGraduationCandidates(CancellationToken cancellationToken)
        {
            var query = new GetGraduationCandidatesQuery();
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Returns general academic statistics dashboard.
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
        {
            var query = new GetStatisticsQuery();
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Executes the end-of-semester Control Engine. 
        /// Calculates SGPA/CGPA, updates academic levels, and applies warnings/dismissals.
        /// </summary>
        [HttpPost("run-engine")]
        [Authorize(Roles = Roles.AdminRole)] // Critical: Only Admins can trigger the engine
        public async Task<IActionResult> RunControlEngine(
            [FromBody] RunEngineRequest request,
            CancellationToken cancellationToken)
        {
            var command = new RunControlEngineCommand(request.Term, request.Year);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            // Returns an analytical summary of what just happened in the database
            return Ok(result.Value);
        }
    }
}