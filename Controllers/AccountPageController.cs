using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OptimizelySAML.Models.Pages;
using OptimizelySAML.Models.ViewModels;

namespace OptimizelySAML.Controllers
{
    public class AccountPageController : PageControllerBase<AccountPage>
    {
        public IActionResult Index(AccountPage currentPage, string returnUrl = "/")
        {
            var model = PageViewModel.Create(currentPage);

            // Example value to store in session
            HttpContext.Session.SetString("returnURL", returnUrl);

            return View(model);
        }

        [HttpPost]
        public IActionResult Login(string provider, string returnUrl = "/")
        {
            // Retrieve the value from session
            var expectedURL = HttpContext.Session.GetString("returnURL");
            if(!string.IsNullOrEmpty(expectedURL))
            {
                returnUrl = expectedURL;
            }

            if (provider == "Saml2Instance1")
            {
                var redirectUrl = Url.Action(nameof(LoginCallback), "AccountPage", new { returnUrl });
                var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
                return Challenge(properties, "Saml2Instance1");
            }
            if (provider == "Saml2Instance2")
            {
                var redirectUrl = Url.Action(nameof(LoginCallback), "AccountPage", new { returnUrl });
                var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
                return Challenge(properties, "Saml2Instance2");
            }

            return View();
        }

        public async Task<IActionResult> LoginCallback(string returnURL = "/")
        {
            if (User.Identity.IsAuthenticated)
            {
                return LocalRedirect(returnURL);
            }
            else
            {
                return LocalRedirect(returnURL);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "AccountPage");
        }
    }
}
