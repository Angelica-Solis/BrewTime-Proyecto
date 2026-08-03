using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrewTime.Web.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private readonly IServicePedido _servicePedido;

        public PedidoController(IServicePedido servicePedido)
        {
            _servicePedido = servicePedido;
        }

        public async Task<IActionResult> Index(DateTime? fecha, int? estadoId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Index", "Login");
            }

            int userId = int.Parse(userIdClaim);

            ViewBag.Estados = await _servicePedido.GetEstadosAsync();
            ViewBag.SelectedFecha = fecha?.ToString("yyyy-MM-dd");
            ViewBag.SelectedEstadoId = estadoId;

            if (userRole == "Administrador" || userRole == "Encargado")
            {
                var pedidos = await _servicePedido.GetTodosPedidosAsync(fecha, estadoId);
                return View("IndexAdmin", pedidos);
            }
            else
            {
                // Cliente or other roles see their own orders
                var pedidos = await _servicePedido.GetHistorialClienteAsync(userId);
                return View("IndexCliente", pedidos);
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Index", "Login");
            }

            var pedido = await _servicePedido.GetDetallePedidoAsync(id);
            if (pedido == null)
            {
                TempData["Error"] = "Pedido no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Security: Client can only view their own order
            if (userRole != "Administrador" && userRole != "Encargado")
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (pedido.ClienteCorreo != userEmail)
                {
                    TempData["Error"] = "No tiene permiso para ver el detalle de este pedido.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(pedido);
        }
    }
}
