using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrewTime.Web.Controllers
{
    [Authorize(Roles = "Cocina")]
    public class CocinaController : Controller
    {
        private readonly IServiceCocina _service;

        public CocinaController(IServiceCocina service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _service.ObtenerPedidosCocinaAsync();

            return View(pedidos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Realizado(int pedidoId)
        {
            var resultado = await _service.MarcarPedidoEnCaminoAsync(pedidoId);

            if (!resultado)
            {
                TempData["Error"] = "No se pudo actualizar el estado del pedido.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "El pedido fue marcado como realizado.";

            return RedirectToAction(nameof(Index));
        }
    }
}

