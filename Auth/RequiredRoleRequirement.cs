using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Owin_Auth.Auth
{
    public class RequiredRoleRequirement : IAuthorizationRequirement
    {
        public int MinimumLevel { get; private set; }

        public RequiredRoleRequirement(int minimumLevel)
        {
            MinimumLevel = minimumLevel;
        }
    }
}