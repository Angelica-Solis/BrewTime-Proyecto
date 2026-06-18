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
            var lista = await _serviceProcesoPreparacion.ListadoProcesosAsync();

            return View(lista);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var detalle = await _serviceProcesoPreparacion.DetailByProductoAsync(id);

            if (detalle == null)
                return NotFound();

            return View(detalle);
        }
    }
}
