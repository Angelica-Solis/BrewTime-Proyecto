using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrewTime.Web.Controllers
{
    
    public class UsuarioController : Controller
    {
        private readonly IServiceUsuario _serviceUsuario;

        public UsuarioController(IServiceUsuario serviceUsuario)
        {
            _serviceUsuario = serviceUsuario;
        }
        // registro de clientes

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegistroClienteDTO());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistroClienteDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var resultado = await _serviceUsuario.RegistrarClienteAsync(dto);

            if (!resultado.Exito)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(dto);
            }

            TempData["Success"] = resultado.Mensaje;

            return RedirectToAction("Index", "Login");
        }

        // listado de usuarios
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Index(string? buscar)
        {
            var collection = await _serviceUsuario.ListAsync();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                collection = collection
                    .Where(u =>
                        u.Nombre.Contains(
                            buscar,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        u.Apellidos.Contains(
                            buscar,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        u.NombreRol.Contains(
                            buscar,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.Buscar = buscar;

            return View(collection);
        }

        // detalle usuario
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var usuario = await _serviceUsuario.FindByIdAsync(id);

            if (usuario == null)
            {
                TempData["Error"] ="El usuario no existe.";

                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        // crear usuario

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _serviceUsuario.ObtenerRolesAdministrativosAsync();

            return View(new UsuarioCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            UsuarioCreateDTO dto)
        {
            ViewBag.Roles = await _serviceUsuario.ObtenerRolesAdministrativosAsync();

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var resultado = await _serviceUsuario.CrearEmpleadoAsync(dto);

            if (!resultado.Exito)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(dto);
            }

            TempData["Success"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        // editar usuario
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _serviceUsuario.ObtenerParaEditarAsync(id);

            if (usuario == null)
            {
                TempData["Error"] = "El usuario no existe.";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new List<string>
            {
                "Cliente",
                "Encargado",
                "Cocina"
            };

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(UsuarioEditDTO dto)
        {
            ViewBag.Roles = new List<string>
            {
                "Cliente",
                "Encargado",
                "Cocina"
            };

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var resultado = await _serviceUsuario.EditarAsync(dto);

            if (!resultado.Exito)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(dto);
            }

            TempData["Success"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }
    }
}