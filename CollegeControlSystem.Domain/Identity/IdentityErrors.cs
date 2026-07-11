
using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Identity
{
    public static class IdentityErrors
    {
        public static readonly Error InvalidCredentials = new(
            "Identity.InvalidCredentials", "Email or Password is incorrect.");
     
        public static readonly Error InvalidPassword = new(
            "Identity.InvalidPassword", "The provided password is incorrect.");

        public static readonly Error InvalidEmail = 
            new("Identity.InvalidEmail", "The provided email is incorrect.");

        public static readonly Error InvalidToken = new(
            "Identity.InvalidToken", "The provided token is invalid.");

        public static readonly Error PasswordResetFailed = new(
            "Identity.PasswordResetFailed", "Error resetting password.");

        public static readonly Error NotFoundUser = new(
            "Identity.NotFoundUser", "User not found.");

        public static readonly Error UserAlreadyExists = new(
            "Identity.UserAlreadyExists", "User with the given email already exists.");

        public static readonly Error InvalidRole = new(
            "Identity.InvalidRole", "Invalid user role specified.");

        public static readonly Error AdminNotAllowed = new(
             "Identity.AdminNotAllowed", "Admins cannot be created via this endpoint.");

        public static readonly Error RoleFailed = new("Identity.RoleFailed", "Failed to assign role");


    }
}