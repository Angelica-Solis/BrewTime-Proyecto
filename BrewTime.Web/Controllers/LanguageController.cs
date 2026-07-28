using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace BrewTime.Web.Controllers
{
    public class LanguageController : Controller
    {
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            // Guarda el idioma en una cookie persistente por 1 año
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
