using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrewTime.Web.Controllers
{
    public class ComboController : Controller
    {
        private readonly IServiceCombo _serviceCombo;

        public ComboController(IServiceCombo serviceCombo)
        {
            _serviceCombo = serviceCombo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceCombo.ListAsync();
            return View(collection);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var combo = await _serviceCombo.FindByIdAsync(id);
            return View(combo);
        }
        [HttpGet]
        public async Task<IActionResult> Maintenance()
        {
            var activos = await _serviceCombo.ListAsync();
            var inactivos = await _serviceCombo.ListInactivosAsync();

            ViewBag.Inactivos = inactivos;
            return View(activos);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            await _serviceCombo.ToggleActivoAsync(id);
            return RedirectToAction(nameof(Maintenance));
        }
    }
}