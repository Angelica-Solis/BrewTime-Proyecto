using BrewTime.Application.Services.Implementations;
using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BrewTime.Web.Controllers
{
    public class ProcesoPreparacionController : Controller
    {
        private readonly IServiceProcesoPreparacion _serviceProcesoPreparacion;

        public ProcesoPreparacionController(IServiceProcesoPreparacion service)
        {
            _serviceProcesoPreparacion = service;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceProcesoPreparacion.ListAsync();
            return View(collection);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var proceso = await _serviceProcesoPreparacion.FindByIdAsync(id);
            return View(proceso);
        }
    }
}
