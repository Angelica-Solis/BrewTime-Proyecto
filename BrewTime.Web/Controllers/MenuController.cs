using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BrewTime.Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IServiceMenu _serviceMenu;
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceCombo _serviceCombo;

        public MenuController(
            IServiceMenu serviceMenu,
            IServiceProducto serviceProducto,
            IServiceCombo serviceCombo)
        {
            _serviceMenu = serviceMenu;
            _serviceProducto = serviceProducto;
            _serviceCombo = serviceCombo;
        }

        // ── Público ──────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceMenu.ListAsync();

            return View(collection);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var menu = await _serviceMenu.FindByIdAsync(id);

            if (menu == null)
                return NotFound();

            return View(menu);
        }

        [HttpGet]
        public async Task<IActionResult> Disponible()
        {
            var ahora = DateTime.Now;

            var fechaActual =
                DateOnly.FromDateTime(ahora);

            var horaActual =
                TimeOnly.FromDateTime(ahora);

            var menu = await _serviceMenu.FindDisponibleAsync(
                fechaActual,
                horaActual);

            
             //La vista recibe un solo MenuDTO,no una colección
          
            return View(menu);
        }

        // ── Mantenimiento ────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> Maintenance()
        {
            
            // ListAsync devuelve todos los menús, ordenados por fecha desde el repositorio
             
            var collection = await _serviceMenu.ListAsync();

            return View(collection);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarDatosFormularioAsync();

            var dto = new MenuFormDTO
            {
                Activo = true,
                FechaInicio =
                    DateOnly.FromDateTime(DateTime.Today),
                FechaFin =
                    DateOnly.FromDateTime(DateTime.Today),
                HoraInicio = new TimeOnly(6, 0),
                HoraFin = new TimeOnly(20, 0)
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuFormDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Nombre))
            {
                bool nombreExiste =
                    await _serviceMenu.ExisteNombreAsync(dto.Nombre);

                if (nombreExiste)
                {
                    ModelState.AddModelError(
                        nameof(dto.Nombre),
                        "Ya existe un menú registrado con este nombre");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["MostrarErrorValidacion"] = true;

                await CargarDatosFormularioAsync();
                return View(dto);
            }

            try
            {
                await _serviceMenu.CreateAsync(dto);

                TempData["MensajeExito"] =
                    "El menú se agregó correctamente";

                TempData["TituloMensaje"] =
                    "Menú registrado";

                return RedirectToAction(nameof(Maintenance));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No fue posible registrar el menú");

                ViewData["MostrarErrorValidacion"] = true;

                await CargarDatosFormularioAsync();
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _serviceMenu.FindFormByIdAsync(id);

            if (dto == null)
                return NotFound();

            await CargarDatosFormularioAsync();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MenuFormDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Nombre))
            {
                bool nombreExiste =
                    await _serviceMenu.ExisteNombreAsync(
                        dto.Nombre,
                        dto.MenuId);

                if (nombreExiste)
                {
                    ModelState.AddModelError(
                        nameof(dto.Nombre),
                        "Ya existe otro menú registrado con este nombre");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["MostrarErrorValidacion"] = true;

                await CargarDatosFormularioAsync();
                return View(dto);
            }

            try
            {
                await _serviceMenu.UpdateAsync(dto);

                TempData["MensajeExito"] =
                    "Los datos del menú se modificaron correctamente";

                TempData["TituloMensaje"] =
                    "Menú modificado";

                return RedirectToAction(nameof(Maintenance));
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                ViewData["MostrarErrorValidacion"] = true;

                await CargarDatosFormularioAsync();
                return View(dto);
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No fue posible modificar el menú");

                ViewData["MostrarErrorValidacion"] = true;

                await CargarDatosFormularioAsync();
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            var menu = await _serviceMenu.FindByIdAsync(id);

            if (menu == null)
            {
                TempData["MensajeError"] =
                    "El menú seleccionado no existe";

                TempData["TituloMensaje"] =
                    "No se pudo completar la operación";

                return RedirectToAction(nameof(Maintenance));
            }

            try
            {
                bool estabaActivo = menu.Activo;

                await _serviceMenu.ToggleActivoAsync(id);

                TempData["TituloMensaje"] = estabaActivo
                    ? "Menú desactivado"
                    : "Menú activado";

                TempData["MensajeExito"] = estabaActivo
                    ? $"El menú \"{menu.Nombre}\" se desactivó correctamente"
                    : $"El menú \"{menu.Nombre}\" se activó correctamente";
            }
            catch (Exception)
            {
                TempData["TituloMensaje"] =
                    "Error";

                TempData["MensajeError"] =
                    "No fue posible cambiar el estado del menú";
            }

            return RedirectToAction(nameof(Maintenance));
        }

        // ── Helper ───────────────────────────────────────────

        private async Task CargarDatosFormularioAsync()
        {
        
             //IServiceProducto.ListAsync devuelve únicamente productos activos según RepositoryProducto
       
            var productos = await _serviceProducto.ListAsync();

         
            //IServiceCombo.ListAsync debe devolver los combos activos, igual que el mantenimiento de Combo
             
            var combos = await _serviceCombo.ListAsync();

            ViewBag.ProductosDisponibles = productos
                .OrderBy(x => x.CategoriaNombre)
                .ThenBy(x => x.Nombre)
                .ToList();

            ViewBag.CombosDisponibles = combos
                .OrderBy(x => x.Nombre)
                .ToList();
        }
    }
}
