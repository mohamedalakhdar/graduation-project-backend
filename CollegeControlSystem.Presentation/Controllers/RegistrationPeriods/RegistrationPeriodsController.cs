using CollegeControlSystem.Application.RegistrationPeriods.CreateRegistrationPeriod;
using CollegeControlSystem.Application.RegistrationPeriods.DeleteRegistrationPeriod;
using CollegeControlSystem.Application.RegistrationPeriods.GetAllRegistrationPeriods;
using CollegeControlSystem.Application.RegistrationPeriods.GetCurrentRegistrationPeriod;
using CollegeControlSystem.Application.RegistrationPeriods.UpdateRegistrationPeriod;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Domain.RegistrationPeriods;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeControlSystem.Presentation.Controllers.RegistrationPeriods
{
    [ApiController]
    [Route("api/registration-periods")]
    public sealed class RegistrationPeriodsController : ControllerBase
    {
        private readonly ISender _sender;

        public RegistrationPeriodsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> Create(
            [FromBody] CreateRegistrationPeriodRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateRegistrationPeriodCommand(
                request.Name,
                request.StartDateUtc,
                request.EndDateUtc,
                request.IsActive,
                request.Term,
                request.Year);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetCurrent), new { id = result.Value }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateRegistrationPeriodRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateRegistrationPeriodCommand(
                id,
                request.Name,
                request.StartDateUtc,
                request.EndDateUtc,
                request.IsActive,
                request.Term,
                request.Year);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == RegistrationPeriodErrors.NotFound)
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }

            return NoContent();
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetCurrentRegistrationPeriodQuery(), cancellationToken);

            if (result.IsFailure)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAllRegistrationPeriodsQuery(), cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeleteRegistrationPeriodCommand(id), cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == RegistrationPeriodErrors.NotFound)
                    return NotFound(result.Error);

                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}
