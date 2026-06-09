using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrewTime.Web.Controllers
{
    public class MenuProductoController : Controller
    {
        private readonly IServiceMenuProducto _service;

        public MenuProductoController(IServiceMenuProducto service)
        {
            _service = service;
        }

        public async Task<ActionResult> Index()
        {
            var collection = await _service.ListAsync();
            return View(collection);
        }

        public async Task<ActionResult> Details(int menuId, int productoId)
        {
            var item = await _service.FindByIdAsync(menuId, productoId);
            return View(item);
        }
    }
}
