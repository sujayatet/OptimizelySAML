using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace OptimizelySAML.Middleware
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly Injected<IPageRouteHelper> _pageRouteHelper;
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            PageData currentPage = _pageRouteHelper.Service.Page;
            if (currentPage == null)
            {
                context.RedirectUri = "/en/accountlogin";
            }

            return base.RedirectToLogin(context);
        }
    }
}
