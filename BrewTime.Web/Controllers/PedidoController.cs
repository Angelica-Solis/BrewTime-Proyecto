using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BrewTime.Application.DTOs;

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

            int userId = int.Parse(userIdClaim);

            var pedido = await _servicePedido.GetDetallePedidoAsync(id);

            if (pedido == null)
            {
                TempData["Error"] = "Pedido no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Seguridad: un cliente solo puede ver sus propios pedidos
            if (userRole != "Administrador" &&
                userRole != "Encargado" &&
                pedido.ClienteId != userId)
            {
                TempData["Error"] = "No tiene permiso para ver el detalle de este pedido.";
                return RedirectToAction(nameof(Index));
            }

            return View(pedido);

        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            try
            {
                var dto = await _servicePedido.PrepararRegistroAsync(UsuarioActual, RolActual);
                return View(dto);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction("Index", "Carrito");
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction("Index", "Carrito");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(PedidoCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    await RecargarRegistroAsync(dto);
                    return View(dto);
                }
                catch (Exception)
                {
                    TempData["Error"] = "No fue posible volver a cargar " + "la información del pedido";

                    return RedirectToAction("Index", "Carrito");
                }
            }

            try
            {
                int pedidoId = await _servicePedido.RegistrarDesdeCarritoAsync(dto, UsuarioActual, RolActual);
                TempData["Success"] = "El pedido se registró correctamente. Ahora puedes continuar con el pago";
                return RedirectToAction(nameof(Detail),new { id = pedidoId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await RecargarRegistroAsync(dto);
                return View(dto);
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await RecargarRegistroAsync(dto);
                return View(dto);
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty, "Ocurrió un error al registrar el pedido");

                await RecargarRegistroAsync(dto);

                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Pago(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "El pedido indicado no es válido";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                var model = await _servicePedido.PrepararPagoAsync(id, UsuarioActual, RolActual);

                if (model == null)
                {
                    TempData["Error"] = "El pedido seleccionado no existe";

                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(nameof(Detail), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pago(PagoViewDTO model)
        {
            PagoPedidoDTO dto = model.Pago;

            if (dto == null || dto.PedidoId <= 0)
            {
                TempData["Error"] = "El pedido indicado no es válido";

                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return await RecargarPagoAsync(dto);
            }

            try
            {
                await _servicePedido.ProcesarPagoAsync(dto, UsuarioActual, RolActual);

                TempData["Success"] = "El pago se procesó correctamente";

                return RedirectToAction(nameof(Detail), new { id = dto.PedidoId });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return await RecargarPagoAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return await RecargarPagoAsync(dto);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al procesar el pago");

                return await RecargarPagoAsync(dto);
            }
        }


        //metodos helpers
        private async Task RecargarRegistroAsync(PedidoCreateDTO dto)
        {

           //se vuelve a consultar el carrito y toda la información proveniente de la base de datos.
            var reconstruido = await _servicePedido.PrepararRegistroAsync(UsuarioActual, RolActual);

            //conservamos únicamente las observaciones escritas por el usuario.
            var observaciones =
                (dto.Detalles ??
                 new List<PedidoLineaCreateDTO>())
                .Where(d => d.CarritoId > 0)
                .GroupBy(d => d.CarritoId)
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Last().Observaciones);

            dto.EsClienteLogueado = reconstruido.EsClienteLogueado;
            dto.Fecha = reconstruido.Fecha;
            dto.EstadoNombre = reconstruido.EstadoNombre;
            dto.EncargadoNombre = reconstruido.EncargadoNombre;
            dto.ClientesDisponibles = reconstruido.ClientesDisponibles;
            dto.MetodosEntrega = reconstruido.MetodosEntrega;

            //reconstruimos los detalles desde el carrito
            dto.Detalles = reconstruido.Detalles;

            foreach (var detalle in dto.Detalles)
            {
                if (observaciones.TryGetValue(detalle.CarritoId, out string? observacion))
                {
                    detalle.Observaciones = observacion;
                }
            }

            //si el usuario es cliente, sus datos se establecen automáticamente
            if (dto.EsClienteLogueado)
            {
                dto.ClienteId = reconstruido.ClienteId;

                dto.ClienteNombre = reconstruido.ClienteNombre;

                dto.ClienteCorreo = reconstruido.ClienteCorreo;
            }
            else
            {
                //si es encargado o administrador, conservamos el cliente seleccionado
                var clienteSeleccionado = dto.ClientesDisponibles.FirstOrDefault(c => c.UsuarioId == dto.ClienteId);

                if (clienteSeleccionado != null)
                {
                    dto.ClienteNombre = $"{clienteSeleccionado.Nombre} " + $"{clienteSeleccionado.Apellidos}";
                    dto.ClienteCorreo = clienteSeleccionado.Correo;
                }
            }

            //se vuelve a consultar el costo real del método seleccionado
            var metodoSeleccionado = dto.MetodosEntrega.FirstOrDefault(m => m.MetodoId == dto.MetodoEntregaId);

            dto.MetodoEntregaNombre = metodoSeleccionado?.Nombre ?? string.Empty;

            dto.CostoEnvio = metodoSeleccionado?.Costo ?? 0;
        }

        private int UsuarioActual
        {
            get
            {
                return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            }
        }

        private string RolActual
        {
            get
            {
                return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            }
        }

        private async Task<IActionResult> RecargarPagoAsync(PagoPedidoDTO dto)
        {
            try
            {
                var model = await _servicePedido.PrepararPagoAsync(dto.PedidoId, UsuarioActual, RolActual);

                if (model == null)
                {
                    TempData["Error"] = "El pedido seleccionado no existe";
                    return RedirectToAction(nameof(Index));
                }

                //conserva los datos introducidos por el usuario, mientras que el detalle y los métodos de pago vuelven a obtenerse desde la base de datos
                model.Pago = dto;

                model.Pago.TotalPedido = model.Pedido.Total;

                return View("Pago", model);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(nameof(Detail), new { id = dto.PedidoId });
            }
        }
    }
}
