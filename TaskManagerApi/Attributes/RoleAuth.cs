using Microsoft.AspNetCore.Authorization;

namespace TaskManager.Api.Attribute
{
    public class AuthRequirement : IAuthorizationRequirement
    {
        private readonly string _routeName;
    }
}
