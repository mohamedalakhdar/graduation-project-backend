using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Identity.LockUnLock
{
    public record LockUnLockCommand(string userId) : ICommand<string>;
}
