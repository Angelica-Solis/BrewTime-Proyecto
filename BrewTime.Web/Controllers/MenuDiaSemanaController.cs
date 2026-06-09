using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrewTime.Web.Controllers
{
    public class MenuDiaSemanaController : Controller
    {
        private readonly IServiceMenuDiaSemana _service;

        public MenuDiaSemanaController(IServiceMenuDiaSemana service)
        {
            _service = service;
        }

        public async Task<ActionResult> Index()
        {
            var collection = await _service.ListAsync();
            return View(collection);
        }

        public async Task<ActionResult> Details(int menuId, byte diaSemana)
        {
            var item = await _service.FindByIdAsync(menuId, diaSemana);
            return View(item);
        }
    }
}
