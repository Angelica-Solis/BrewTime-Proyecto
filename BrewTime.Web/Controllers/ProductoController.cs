using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BrewTime.Web.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceCategoria _serviceCategoria;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoController(
            IServiceProducto serviceProducto,
            IServiceCategoria serviceCategoria,
            IWebHostEnvironment webHostEnvironment)
        {
            _serviceProducto = serviceProducto;
            _serviceCategoria = serviceCategoria;
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task CargarCategoriasAsync()
        {
            var categorias = await _serviceCategoria.ListActivasAsync();

            ViewBag.Categorias = categorias.Select(c => new SelectListItem
            {
                Value = c.CategoriaID.ToString(),
                Text = c.Nombre
            }).ToList();
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
            var activos = await _serviceProducto.ListAsync();
            var inactivos = await _serviceProducto.ListInactivosAsync();

            ViewBag.Inactivos = inactivos;
            return View(activos);   // Model = activos, ViewBag.Inactivos = inactivos
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarCategoriasAsync();
            return View(new ProductoFormDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoFormDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategoriasAsync();
                return View(dto);
            }

            await _serviceProducto.CreateAsync(dto, _webHostEnvironment.WebRootPath);
            return RedirectToAction(nameof(Maintenance));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _serviceProducto.FindFormByIdAsync(id);
            await CargarCategoriasAsync();
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductoFormDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategoriasAsync();
                return View(dto);
            }

            await _serviceProducto.UpdateAsync(dto, _webHostEnvironment.WebRootPath);
            return RedirectToAction(nameof(Maintenance));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            await _serviceProducto.ToggleActivoAsync(id);
            return RedirectToAction(nameof(Maintenance));
        }
    }
}
