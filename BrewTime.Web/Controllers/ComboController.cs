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
    }
}