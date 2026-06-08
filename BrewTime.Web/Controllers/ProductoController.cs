using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrewTime.Web.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IServiceProducto _serviceProducto;

        public ProductoController(IServiceProducto serviceProducto)
        {
            _serviceProducto = serviceProducto;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceProducto.ListAsync();
            return View(collection);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var producto = await _serviceProducto.FindByIdAsync(id);
            return View(producto);
        }
    }
}