using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrewTime.Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IServiceMenu _serviceMenu;

        public MenuController(IServiceMenu serviceMenu)
        {
            _serviceMenu = serviceMenu;
        }

        public async Task<ActionResult> Index()
        {
            ViewData["Title"] = "Menús";
            var collection = await _serviceMenu.ListAsync();
            return View(collection);
        }

        public async Task<ActionResult> Details(int id)
        {
            var menu = await _serviceMenu.FindByIdAsync(id);
            return View(menu);
        }
    }
}
