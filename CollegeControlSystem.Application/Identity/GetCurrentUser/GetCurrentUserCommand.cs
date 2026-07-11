using CollegeControlSystem.Application.Abstractions.Messaging;
using System.Security.Claims;

namespace CollegeControlSystem.Application.Identity.GetCurrentUser
{
    public record GetCurrentUserQuery(ClaimsPrincipal userPrincipal) : IQuery<GetCurrentUserResponse>;
}
