using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CollegeControlSystem.Application.Identity.GetCurrentUser
{
    internal class GetCurrentUserQueryHandler :IQueryHandler<GetCurrentUserQuery, GetCurrentUserResponse>
    {
        private readonly UserManager<AppUser> userManager;

        public GetCurrentUserQueryHandler(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<Result<GetCurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            // ⚠ Important Note:
            // userManager.GetUserId(userPrincipal) expects a Claim with NameIdentifier.
            // In our JWT, the UserId is stored in a custom claim: Keys.UserIdKey ("UserId").
            // So we must use FindFirstValue(Keys.UserIdKey) to get the actual userId.
            //var userId = userManager.GetUserId(userPrincipal);

            // 1️⃣ احصل على UserId من الـ JWT Claim الصحيح
            var userId = request.userPrincipal.FindFirstValue(Keys.UserIdKey); // Keys.UserIdKey = "UserId"
            var role = request.userPrincipal.FindFirstValue(Keys.RolesKey);

            if (string.IsNullOrEmpty(userId))
                return Result<GetCurrentUserResponse>.Failure(IdentityErrors.NotFoundUser);

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return Result<GetCurrentUserResponse>.Failure(IdentityErrors.NotFoundUser);

            var userResponse = new GetCurrentUserResponse
            (
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                role
            );
            return Result<GetCurrentUserResponse>.Success(userResponse);
        }
    }
}
