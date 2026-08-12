using System.Security.Claims;
using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrewTime.Web.Controllers
{
    [Authorize(Roles = "Cliente,Encargado")]
    public class CarritoController : Controller
    {
        private readonly IServiceCarrito _service;

        public CarritoController(IServiceCarrito service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var carrito = await _service.ObtenerCarritoAsync(UsuarioActual);

            return View(carrito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarProducto(int productoId, int cantidad)
        {
            await _service.AgregarProductoAsync(
                UsuarioActual,
                productoId,
                cantidad);

            var carrito = await _service.ObtenerCarritoAsync(UsuarioActual);

            return Json(new
            {
                ok = true,
                mensaje = "Producto agregado al carrito",
                cantidad = carrito.Items.Sum(x => x.Cantidad),
                subtotal = carrito.Subtotal,
                total = carrito.Total
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarCombo(int comboId, int cantidad)
        {
            await _service.AgregarComboAsync(
                UsuarioActual,
                comboId,
                cantidad);

            var carrito = await _service.ObtenerCarritoAsync(UsuarioActual);

            return Json(new
            {
                ok = true,
                mensaje = "Combo agregado al carrito",
                cantidad = carrito.Items.Sum(x => x.Cantidad),
                subtotal = carrito.Subtotal,
                total = carrito.Total
            });
        }
        [HttpPost]
        public async Task<IActionResult> ActualizarCantidad(int carritoId,int cantidad)
        {
            await _service.ActualizarCantidadAsync(
                carritoId,
                cantidad);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int carritoId)
        {
            await _service.EliminarAsync(carritoId);

            TempData["Success"] = "Producto eliminado.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Vaciar()
        {
            await _service.VaciarAsync(UsuarioActual);

            TempData["Success"] = "Carrito vaciado.";

            return RedirectToAction(nameof(Index));
        }
        private int UsuarioActual
        {   
            get
            {
                return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            }
        }
    }
}
