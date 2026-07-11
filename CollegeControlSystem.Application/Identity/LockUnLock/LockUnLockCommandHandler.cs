using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollegeControlSystem.Application.Identity.LockUnLock
{
    internal class LockUnLockCommandHandler : ICommandHandler<LockUnLockCommand, string>
    {
        private readonly UserManager<AppUser> userManager;

        public LockUnLockCommandHandler(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<Result<string>> Handle(LockUnLockCommand request, CancellationToken cancellationToken)
        {
            // Find the user by their Id
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.userId);
            if (user == null)
                return Result<string>.Failure(IdentityErrors.NotFoundUser);

            var message = "";
            // If user is not locked out, lock them for 1 year
            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.UtcNow)
            {
                user.LockoutEnd = DateTime.UtcNow.AddYears(1);  // Lock the user
                message = "User Locked Ssuccessfully";
            }
            else
            {
                // Unlock the user by setting LockoutEnd to null
                user.LockoutEnd = null;
                message = "User UnLocked Ssuccessfully";

            }

            // Update the user in the database
            await userManager.UpdateAsync(user);

            return Result<string>.Success(message);

        }
      }
}
