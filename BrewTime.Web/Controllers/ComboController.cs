using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BrewTime.Web.Controllers
{
    public class ComboController : Controller
    {
        private readonly IServiceCombo _serviceCombo;
        private readonly IServiceCategoria _serviceCategoria;
        private readonly IServiceProducto _serviceProducto;

        public ComboController(
            IServiceCombo serviceCombo,
            IServiceCategoria serviceCategoria,
            IServiceProducto serviceProducto)
        {
            _serviceCombo = serviceCombo;
            _serviceCategoria = serviceCategoria;
            _serviceProducto = serviceProducto;
        }

        // ── Público ──────────────────────────────────────────

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

        // ── Mantenimiento ─────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Maintenance()
        {
            var activos = await _serviceCombo.ListAsync();
            var inactivos = await _serviceCombo.ListInactivosAsync();
            ViewBag.Inactivos = inactivos;
            return View(activos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarDatosFormulario(null);
            return View(new ComboFormDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ComboFormDTO dto)
        {
            ModelState.Remove("ProductosSeleccionados");

            if (!ModelState.IsValid)
            {
                await CargarDatosFormulario(dto);
                return View(dto);
            }

            await _serviceCombo.CreateAsync(dto);
            return RedirectToAction(nameof(Maintenance));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _serviceCombo.FindFormByIdAsync(id);
            await CargarDatosFormulario(dto);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ComboFormDTO dto)
        {
            ModelState.Remove("ProductosSeleccionados");

            if (!ModelState.IsValid)
            {
                await CargarDatosFormulario(dto);
                return View(dto);
            }

            await _serviceCombo.UpdateAsync(dto);
            return RedirectToAction(nameof(Maintenance));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            await _serviceCombo.ToggleActivoAsync(id);
            return RedirectToAction(nameof(Maintenance));
        }

        // ── Helper ───────────────────────────────────────────

        private async Task CargarDatosFormulario(ComboFormDTO? dto)
        {
            // Categorías para el select
            var categorias = await _serviceCategoria.ListActivasAsync();
            ViewBag.Categorias = new SelectList(categorias, "CategoriaID", "Nombre");

            // Todos los productos activos
            var productos = await _serviceProducto.ListAsync();

            // Marcar los que ya pertenecen al combo (en Edit)
            var productosYaEnCombo = dto?.ProductosSeleccionados ?? new List<ComboProductoFormDTO>();

            ViewBag.ProductosDisponibles = productos.Select(p => new ComboProductoFormDTO
            {
                ProductoID = p.ProductoID,
                NombreProducto = p.Nombre,
                Cantidad = productosYaEnCombo
                                    .FirstOrDefault(x => x.ProductoID == p.ProductoID)?.Cantidad ?? 1,
                Seleccionado = productosYaEnCombo
                                    .Any(x => x.ProductoID == p.ProductoID)
            }).ToList();
        }
    }
}