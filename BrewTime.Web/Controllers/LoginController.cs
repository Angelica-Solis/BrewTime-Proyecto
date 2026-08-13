using System.Security.Claims;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BrewTime.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IServiceAutenticacion _service;

        public LoginController(IServiceAutenticacion service)
        {
            _service = service;
        }
        // get
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginDTO());
        }

        // post
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var resultado = await _service.LoginAsync(dto);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensaje);
                return View(dto);
            }

            var usuario = resultado.Usuario;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre + " " + usuario.Apellidos),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            TempData["Success"] = $"¡Bienvenido(a), {usuario.Nombre}!";

            return RedirectToAction("Index", "Home");
        }

        //logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Success"] = "La sesión se cerró correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Cambiar el usuario para la verificacion
        [HttpPost]
        public async Task<IActionResult> SwitchUser(int usuarioId)
        {
            var usuario = await _service.GetUsuarioByIdAsync(usuarioId);
            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Index", "Home");
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre + " " + usuario.Apellidos),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            TempData["Success"] = $"¡Sesión cambiada a {usuario.Nombre} ({usuario.Rol.Nombre})!";

            return RedirectToAction("Index", "Home");
        }
    }
}

