using BrewTime.Application.DTOs;
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
        [HttpGet]
        public async Task<IActionResult> Maintenance()
        {
            var collection = await _serviceProducto.ListAsync();
            return View(collection);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new ProductoFormDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoFormDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _serviceProducto.CreateAsync(dto);
            return RedirectToAction(nameof(Maintenance));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _serviceProducto.FindFormByIdAsync(id);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductoFormDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _serviceProducto.UpdateAsync(dto);
            return RedirectToAction(nameof(Maintenance));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _serviceProducto.DeleteAsync(id);
            return RedirectToAction(nameof(Maintenance));
        }
    }
}