using System.Security.Claims;

namespace OptimizelySAML.Middleware
{
    public class PageAccessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PageAccessMiddleware> _logger;

        public PageAccessMiddleware(RequestDelegate next, ILogger<PageAccessMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Add page-specific access restrictions here
            var path = context.Request.Path.Value;
            if (IsRestrictedPage(path) && !UserHasAccess(context.User, path))
            {
                context.Response.Redirect("/en/azureloginpage");
                return;
            }

            if (!context.User.Identity.IsAuthenticated)
            {
                // Redirect unauthenticated users to the login page
                context.Response.Redirect("/login?ReturnUrl=" + context.Request.Path);
                return;
            }

            await _next(context);
        }

        private bool IsRestrictedPage(string path)
        {
            // Define your logic to identify restricted pages
            var restrictedPages = new[] { "/restrictedpage" };
            return restrictedPages.Contains(path);
        }

        private bool UserHasAccess(ClaimsPrincipal user, string path)
        {
            // Implement your logic to verify if the user has access
            // This could be based on roles, claims, etc.
            return user.IsInRole("Administrator"); // Example check
        }
    }
}
