using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrewTime.Web.Controllers
{
    public class MenuComboController : Controller
    {
        private readonly IServiceMenuCombo _service;

        public MenuComboController(IServiceMenuCombo service)
        {
            _service = service;
        }

        public async Task<ActionResult> Index()
        {
            var collection = await _service.ListAsync();
            return View(collection);
        }

        public async Task<ActionResult> Details(int menuId, int comboId)
        {
            var item = await _service.FindByIdAsync(menuId, comboId);
            return View(item);
        }
    }
}
