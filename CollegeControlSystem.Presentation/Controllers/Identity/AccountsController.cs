using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Application.Identity.Login;
using CollegeControlSystem.Application.Identity.RefreshToken;
using CollegeControlSystem.Application.Identity.RevokeToken;
using CollegeControlSystem.Application.Identity.ForgetPassword;
using CollegeControlSystem.Application.Identity.ResetPassword;
using CollegeControlSystem.Application.Identity.LockUnLock;
using CollegeControlSystem.Application.Identity.GetCurrentUser;

namespace CollegeControlSystem.Presentation.Controllers.Identity
{
    [Route("api/accounts")]
    [ApiController]
    public sealed class AccountsController : ControllerBase
    {
        private readonly ISender _sender;

        public AccountsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Gets the currently authenticated user's profile. Requires authentication.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var query = new GetCurrentUserQuery(User);
            var result = await _sender.Send(query);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Authenticates a user and returns JWT and refresh tokens. No authentication required.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return Unauthorized(result.Error); // 401 Unauthorized for bad credentials
            }

            return Ok(result.Value);
        }

        //[HttpPost("register")]
        //[Authorize(Roles = "Admin")] // Strict RBAC: Only Admins can create new users in a college system
        //public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        //{
        //    var command = new RegisterCommand(
        //        request.UserName,
        //        request.Email,
        //        request.Password,
        //        request.PhoneNumber,
        //        request.Role,
        //        request.AcademicNumber,
        //        request.NationalId,
        //        request.ProgramId,
        //        request.DepartmentId,
        //        request.Degree
        //        );

        //    var result = await _sender.Send(command, cancellationToken);

        //    if (result.IsFailure)
        //    {
        //        if (result.Error == IdentityErrors.UserAlreadyExists)
        //        {
        //            return Conflict(result.Error); // 409 Conflict
        //        }
        //        return BadRequest(result.Error);
        //    }

        //    // Return 200 OK with Auth Response (Token) so the admin/user can use it immediately if needed
        //    return Ok(result.Value);
        //}

        /// <summary>
        /// Refreshes an expired JWT using a valid refresh token. No authentication required.
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous] // Needs to be anonymous because the Access Token might already be expired
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var command = new RefreshTokenCommand(request.RefreshToken);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error); // Or 401 if you prefer strict auth errors
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Revokes a refresh token (logout). Requires authentication.
        /// </summary>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
        {
            // Revoking token (Log out)
            var command = new RevokeTokenCommand(request.RefreshToken);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Sends a password reset email. No authentication required.
        /// </summary>
        [HttpPost("forget-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request, CancellationToken cancellationToken)
        {
            // BaseUrl is passed from client to ensure the email link points to the correct frontend URL
            var command = new ForgetPasswordCommand(request.Email, request.BaseUrl);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                // Security Note: Sometimes it's better to return OK even if email is not found to prevent enumeration
                return BadRequest(result.Error);
            }

            return Ok(new { message = result.Value });
        }

        /// <summary>
        /// Resets a user's password using a reset token. No authentication required.
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var command = new ResetPasswordCommand(request.UserId, request.Code, request.NewPassword);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(new { message = result.Value });
        }

        /// <summary>
        /// Locks or unlocks a user account. Requires Admin role.
        /// </summary>
        [HttpPut("lock-unlock/{userId:int}")] // Assuming UserID is int based on TPT Schema
        [Authorize(Roles = Roles.AdminRole)]
        public async Task<IActionResult> LockUnlockUser(LockUnlockRequest request, CancellationToken cancellationToken)
        {
            var command = new LockUnLockCommand(request.UserId);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                if (result.Error == IdentityErrors.NotFoundUser)
                {
                    return NotFound(result.Error);
                }
                return BadRequest(result.Error);
            }

            return Ok(new { message = result.Value });
        }
    }
}
