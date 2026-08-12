using System.Diagnostics;
using BrewTime.Web.Models;
using Microsoft.AspNetCore.Mvc;
using BrewTime.Application.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace BrewTime.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServicePedido _servicePedido;

        public HomeController(ILogger<HomeController> logger, IServicePedido servicePedido)
        {
            _logger = logger;
            _servicePedido = servicePedido;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    int userId = int.Parse(userIdClaim);
                    var historial = await _servicePedido.GetHistorialClienteAsync(userId);
                    
                    // Filtrar pedidos activos: Aceptada, En preparación, En camino
                    var pedidosActivos = historial
                        .Where(p => p.EstadoNombre.Equals("Aceptada", StringComparison.OrdinalIgnoreCase) ||
                                    p.EstadoNombre.Contains("preparaci", StringComparison.OrdinalIgnoreCase) ||
                                    p.EstadoNombre.Contains("prepar", StringComparison.OrdinalIgnoreCase) ||
                                    p.EstadoNombre.Equals("En camino", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    ViewBag.PedidosActivos = pedidosActivos;
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
