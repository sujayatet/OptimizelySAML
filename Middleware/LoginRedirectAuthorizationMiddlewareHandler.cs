using Castle.MicroKernel.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using System.Security.Claims;

namespace OptimizelySAML.Middleware
{
    public class LoginRedirectAuthorizationMiddlewareHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            // Add page-specific access restrictions here
            
            var path = context.Request.Path.Value;
            if (IsRestrictedPage(path) && !UserHasAccess(context.User, path))
            {
                context.Response.Redirect("/en/accountlogin");
            }

            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }

        private bool IsRestrictedPage(string path)
        {
            // Define your logic to identify restricted pages
            var restrictedPages = new[] { "/en/restrictedpage/" };
            return restrictedPages.Contains(path);
        }

        private bool UserHasAccess(ClaimsPrincipal user, string path)
        {
            // Implement your logic to verify if the user has access
            // This could be based on roles, claims, etc.
            return user.IsInRole("CmsAdmins"); // Example check
        }
    }
}
