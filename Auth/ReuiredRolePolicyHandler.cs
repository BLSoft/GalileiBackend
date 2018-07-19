using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Owin_Auth.Utils;

namespace Owin_Auth.Auth
{
    public class ReuiredRolePolicyHandler: AuthorizationHandler<RequiredRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequiredRoleRequirement requirement)
        {
            if (!context.User.HasClaim(claim =>
                claim.Issuer == Config.Configuration["Tokens:Issuer"] && claim.Type == "Role"))
            {
                return Task.CompletedTask;
            }

            int role;
            if (!int.TryParse(context.User.FindFirst(c =>
                c.Issuer == Config.Configuration["Tokens:Issuer"] && c.Type == "Role").Value,out role))
            {
                return Task.CompletedTask;
            }

            if (role >= requirement.MinimumLevel)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}