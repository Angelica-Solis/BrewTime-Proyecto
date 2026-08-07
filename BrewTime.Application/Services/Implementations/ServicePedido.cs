using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Models;
using BrewTime.Infraestructure.Repository.Implemetations;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrewTime.Application.Services.Implementations
{
    public class ServicePedido : IServicePedido
    {
        private readonly IRepositoryPedido _repository;
        private readonly IRepositoryCarrito _repositoryCarrito;
        private readonly IRepositoryUsuario _repositoryUsuario;
        private readonly IMapper _mapper;

        public ServicePedido(IRepositoryPedido repository, IRepositoryCarrito repositoryCarrito, IRepositoryUsuario repositoryUsuario, IMapper mapper)
        {
            _repository = repository;
            _repositoryCarrito = repositoryCarrito;
            _repositoryUsuario = repositoryUsuario;
            _mapper = mapper;
        }

        public async Task<ICollection<PedidoListDTO>> GetHistorialClienteAsync(int clienteId)
        {
            var list = await _repository.ListByClienteAsync(clienteId);
            return _mapper.Map<ICollection<PedidoListDTO>>(list);
        }

        public async Task<ICollection<PedidoListDTO>> GetTodosPedidosAsync(DateTime? fecha, int? estadoId)
        {
            var list = await _repository.ListAllAsync();

            if (fecha.HasValue)
            {
                list = list.Where(p => p.FechaCreacion.Date == fecha.Value.Date).ToList();
            }
            if (estadoId.HasValue)
            {
                list = list.Where(p => p.EstadoId == estadoId.Value).ToList();
            }

            return _mapper.Map<ICollection<PedidoListDTO>>(list);
        }

        public async Task<PedidoDetalleDTO?> GetDetallePedidoAsync(int pedidoId)
        {
            var pedido = await _repository.FindByIdAsync(pedidoId);

            if (pedido == null)
                return null;

            var detalle = new PedidoDetalleDTO
            {
                PedidoId = pedido.PedidoId,
                Fecha = pedido.FechaCreacion,

                ClienteNombre = $"{pedido.Cliente.Nombre} {pedido.Cliente.Apellidos}",
                ClienteCorreo = pedido.Cliente.Correo,
                ClienteId = pedido.ClienteId,

                Encargado = pedido.Empleado != null ? $"{pedido.Empleado.Nombre} {pedido.Empleado.Apellidos}" : "Sin asignar",

                MetodoEntrega = pedido.MetodoEntrega.Nombre,
                MetodoPago = pedido.MetodoPago != null ? pedido.MetodoPago.Nombre : "No registrado",

                Estado = pedido.Estado.Nombre,
                Subtotal = pedido.Subtotal,
                Impuesto = pedido.Impuesto,
<<<<<<< HEAD

                CostoEnvio = pedido.MetodoEntrega.Costo,

                Total = pedido.Subtotal
                  + pedido.Impuesto
                  + pedido.MetodoEntrega.Costo,

=======
                CostoEnvio = pedido.CostoEnvio,
                Total = pedido.Total,
>>>>>>> def5c594a889f0ae35c70f0bdda871ca0eb4475e
                Detalles = new List<PedidoDetalleLineaDTO>()
            };

            foreach (var item in pedido.PedidoDetalle)
            {
                detalle.Detalles.Add(new PedidoDetalleLineaDTO
                {
                    Producto = item.Producto != null ? item.Producto.Nombre : item.Combo!.Nombre,

                    Precio = item.PrecioUnitario,
                    Cantidad = item.Cantidad,
                    Subtotal = item.Subtotal,
                    Impuesto = pedido.Subtotal > 0 ? Math.Round((item.Subtotal / pedido.Subtotal) * pedido.Impuesto, 2) : 0,
                    Observaciones = item.Observaciones ?? ""
                });
            }

            return detalle;
        }

        public async Task<ICollection<EstadoPedidoDTO>> GetEstadosAsync()
        {
            var estados = await _repository.ListEstadosAsync();
            return _mapper.Map<ICollection<EstadoPedidoDTO>>(estados);
        }

        #region Registrar Pedido
        //registrar pedido
        public async Task<PedidoCreateDTO> PrepararRegistroAsync(int usuarioActualId, string rolActual)
        {
            var usuarioActual = await _repositoryUsuario.FindByIdAsync(usuarioActualId);

            if (usuarioActual == null)
            {
                throw new KeyNotFoundException("No se encontró el usuario actual");
            }

            var carrito = await _repositoryCarrito.GetByUsuarioAsync(usuarioActualId);

            if (carrito == null || !carrito.Any())
            {
                throw new InvalidOperationException("El carrito está vacío");
            }

            var estadoPendiente = await _repository.FindEstadoByNombreAsync("Pendiente de pago");

            if (estadoPendiente == null)
            {
                throw new InvalidOperationException("No se encontró el estado Pendiente de pago");
            }

            var metodosEntrega = await _repository.ListMetodosEntregaAsync();

            if (metodosEntrega == null || !metodosEntrega.Any())
            {
                throw new InvalidOperationException("No existen métodos de entrega registrados");
            }

            bool esCliente = rolActual.Equals("Cliente", StringComparison.OrdinalIgnoreCase);

            var dto = new PedidoCreateDTO
            {
                EsClienteLogueado = esCliente,
                Fecha = DateTime.Now,
                EstadoNombre = estadoPendiente.Nombre,
                EncargadoNombre = esCliente ? string.Empty : $"{usuarioActual.Nombre} " + $"{usuarioActual.Apellidos}",

                MetodosEntrega = metodosEntrega
                    .Select(m => new MetodoEntregaDTO
                    {
                        MetodoId = m.MetodoId,
                        Nombre = m.Nombre,
                        Costo = m.Costo
                    })
                    .ToList(),

                Detalles = carrito.Select(c => CrearLineaPedido(c)).ToList()
            };
            
            //si está logueado un cliente, se establece automáticamente
            if (esCliente)
            {
                dto.ClienteId = usuarioActual.UsuarioId;

                dto.ClienteNombre = $"{usuarioActual.Nombre} " + $"{usuarioActual.Apellidos}";

                dto.ClienteCorreo = usuarioActual.Correo;
            }
            else
            {
                //para encargado o administrador se cargan únicamente los usuarios cliente activos
                var usuarios = await _repositoryUsuario.ListAsync();

                dto.ClientesDisponibles = usuarios
                    .Where(u =>
                        u.Activo &&
                        u.Rol != null &&
                        u.Rol.Nombre.Equals(
                            "Cliente",
                            StringComparison.OrdinalIgnoreCase))
                    .OrderBy(u => u.Nombre)
                    .ThenBy(u => u.Apellidos)
                    .Select(u => new UsuarioDetalleDTO
                    {
                        UsuarioId = u.UsuarioId,
                        NombreRol = u.Rol.Nombre,
                        Nombre = u.Nombre,
                        Apellidos = u.Apellidos,
                        Correo = u.Correo,
                        Telefono = u.Telefono,
                        Activo = u.Activo
                    })
                    .ToList();
            }

            //selecciona inicialmente el método sin costo, normalmente Recogida en tienda
            var metodoInicial = metodosEntrega.OrderBy(m => m.Costo).First();
            dto.MetodoEntregaId = metodoInicial.MetodoId;
            dto.MetodoEntregaNombre = metodoInicial.Nombre;
            dto.CostoEnvio = metodoInicial.Costo;

            return dto;
        }

        private static PedidoLineaCreateDTO CrearLineaPedido(Carrito item)
        {
            if (item.Cantidad <= 0)
            {
                throw new InvalidOperationException("El carrito contiene una cantidad inválida");
            }

            bool tieneProducto = item.ProductoId.HasValue && item.Producto != null;

            bool tieneCombo = item.ComboId.HasValue && item.Combo != null;

            if (tieneProducto == tieneCombo)
            {
                throw new InvalidOperationException("Una línea del carrito debe contener " + "un producto o un combo, pero no ambos");
            }

            decimal precio = tieneProducto ? item.Producto!.Precio : item.Combo!.PrecioEspecial;

            string nombre = tieneProducto ? item.Producto!.Nombre : item.Combo!.Nombre;

            return new PedidoLineaCreateDTO
            {
                CarritoId = item.CarritoId,
                ProductoId = item.ProductoId,
                ComboId = item.ComboId,
                Nombre = nombre,                
                PrecioUnitario = precio,
                Cantidad = item.Cantidad
            };
        }

        public async Task<int> RegistrarDesdeCarritoAsync(PedidoCreateDTO dto, int usuarioActualId, string rolActual)
        {
            var usuarioActual = await _repositoryUsuario.FindByIdAsync(usuarioActualId);

            if (usuarioActual == null)
            {
                throw new KeyNotFoundException("No se encontró el usuario actual");
            }

            var carrito = await _repositoryCarrito.GetByUsuarioAsync(usuarioActualId);

            if (carrito == null || !carrito.Any())
            {
                throw new InvalidOperationException("No es posible registrar el pedido " + "porque el carrito está vacío");
            }

            bool esCliente = rolActual.Equals("Cliente", StringComparison.OrdinalIgnoreCase);

            int clienteId;

            if (esCliente)
            { 
                //el cliente logueado siempre registra el pedido para sí mismo
                clienteId = usuarioActualId;
            }
            else
            {
                //el encargado debe seleccionar al cliente
                if (!dto.ClienteId.HasValue || dto.ClienteId.Value <= 0)
                {
                    throw new InvalidOperationException("Debe seleccionar el cliente del pedido");
                }

                var clienteSeleccionado = await _repositoryUsuario.FindByIdAsync(dto.ClienteId.Value);

                bool esClienteValido =
                    clienteSeleccionado != null &&
                    clienteSeleccionado.Activo &&
                    clienteSeleccionado.Rol != null &&
                    clienteSeleccionado.Rol.Nombre.Equals("Cliente", StringComparison.OrdinalIgnoreCase);

                if (!esClienteValido)
                {
                    throw new InvalidOperationException("El usuario seleccionado no es " + "un cliente válido");
                }

                clienteId = clienteSeleccionado.UsuarioId;
            }

            if (!dto.MetodoEntregaId.HasValue || dto.MetodoEntregaId.Value <= 0)
            {
                throw new InvalidOperationException("Debe seleccionar un método de entrega");
            }

            var metodoEntrega = await _repository.FindMetodoEntregaByIdAsync(dto.MetodoEntregaId.Value);

            if (metodoEntrega == null)
            {
                throw new InvalidOperationException("El método de entrega seleccionado no existe");
            }

            bool esEntregaDomicilio = metodoEntrega.Nombre.Contains("domicilio", StringComparison.OrdinalIgnoreCase);

            if (esEntregaDomicilio && string.IsNullOrWhiteSpace(dto.DireccionEntrega))
            {
                throw new InvalidOperationException("Debe ingresar la dirección de entrega");
            }

            var estadoPendiente = await _repository.FindEstadoByNombreAsync("Pendiente de pago");

            if (estadoPendiente == null)
            {
                throw new InvalidOperationException("No se encontró el estado Pendiente de pago.");
            }

            //las observaciones sí vienen del formulario, pero se relacionan con una línea real del carrito
            var observacionesPorCarrito =
                (dto.Detalles ??
                 new List<PedidoLineaCreateDTO>())
                .GroupBy(d => d.CarritoId)
                .ToDictionary(grupo => grupo.Key, grupo => grupo.Last().Observaciones);

            var detallesPedido = new List<PedidoDetalle>();

            decimal subtotalPedido = 0;
            decimal impuestoPedido = 0;

            foreach (var itemCarrito in carrito)
            {
                if (itemCarrito.Cantidad <= 0)
                {
                    throw new InvalidOperationException("Una línea del carrito contiene " + "una cantidad inválida");
                }

                bool tieneProducto = itemCarrito.ProductoId.HasValue && itemCarrito.Producto != null;

                bool tieneCombo = itemCarrito.ComboId.HasValue && itemCarrito.Combo != null;

                if (tieneProducto == tieneCombo)
                {
                    throw new InvalidOperationException("Una línea del carrito no contiene " + "un producto o combo válido");
                }

                //el precio se consulta nuevamente desde la BD

                decimal precioUnitario = tieneProducto ? itemCarrito.Producto!.Precio : itemCarrito.Combo!.PrecioEspecial;

                decimal subtotalLinea =
                    Math.Round(
                        precioUnitario *
                        itemCarrito.Cantidad,
                        2,
                        MidpointRounding.AwayFromZero);

                decimal impuestoLinea =
                    Math.Round(
                        subtotalLinea * 0.13m,
                        2,
                        MidpointRounding.AwayFromZero);

                observacionesPorCarrito.TryGetValue(itemCarrito.CarritoId, out string? observaciones);

                detallesPedido.Add( new PedidoDetalle
                    {
                        ProductoId = itemCarrito.ProductoId,
                        ComboId = itemCarrito.ComboId,
                        Cantidad = itemCarrito.Cantidad,
                        PrecioUnitario = precioUnitario,
                        Subtotal = subtotalLinea,
                        Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
                    });

                subtotalPedido += subtotalLinea;

                impuestoPedido += impuestoLinea;
            }

            decimal costoEnvio = metodoEntrega.Costo;

            decimal totalPedido = subtotalPedido + impuestoPedido + costoEnvio;

            DateTime ahora = DateTime.Now;

            var pedido = new Pedido
                {
                    ClienteId = clienteId,
                    /*
                     * Si es cliente no hay encargado
                     * Si es encargado o administrador se almacena el usuario logueado
                     */
                    EmpleadoId = esCliente ? null : usuarioActualId,
                    EstadoId = estadoPendiente.EstadoId,
                    MetodoEntregaId = metodoEntrega.MetodoId,

                    //todavía no se ha realizado el pago.
                    MetodoPagoId = null,
                    DireccionEntrega = esEntregaDomicilio ? dto.DireccionEntrega!.Trim() : null,
                    CostoEnvio = costoEnvio,
                    Subtotal = subtotalPedido,
                    Impuesto = impuestoPedido,
                    Total = totalPedido,
                    MontoPagado = null,
                    Vuelto = null,
                    UltimosDigitosTarjeta = null,
                    FechaCreacion = ahora,
                    FechaActualizacion = ahora,
                    PedidoDetalle = detallesPedido,

                    PedidoHistorialEstado = new List<PedidoHistorialEstado> {
                    new PedidoHistorialEstado
                    {
                        EstadoId = estadoPendiente.EstadoId,
                        FechaCambio = ahora,
                        UsuarioId = usuarioActualId
                    }
                        }
                };

            await _repository.CreateAsync(
                pedido);


            //después de registrar correctamente el pedido,se eliminan los artículos del carrito
            await _repositoryCarrito.DeleteAllAsync(usuarioActualId);

            await _repositoryCarrito.SaveChangesAsync();


            //después de SaveChanges, EF Core asigna automáticamente el PedidoId

            return pedido.PedidoId;
        }
        #endregion

        #region Pago del pedido
        //pago del pedido
        public async Task<PagoFormDTO?> PrepararPagoAsync(int pedidoId, int usuarioActualId, string rolActual)
        {
            var pedido = await _repository.FindByIdAsync(pedidoId);

            if (pedido == null)
                return null;

            ValidarAccesoPedido(pedido, usuarioActualId, rolActual);

            if (!pedido.Estado.Nombre.Equals("Pendiente de pago", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Este pedido ya no se encuentra pendiente de pago");
            }

            if (pedido.MetodoPagoId.HasValue)
            {
                throw new InvalidOperationException("El pago de este pedido ya fue registrado");
            }

            var metodosPago =await _repository.ListMetodosPagoAsync();

            if (metodosPago == null || !metodosPago.Any())
            {
                throw new InvalidOperationException("No existen métodos de pago registrados");
            }


            //reutilizamos el método existente para obtener el desglose completo del pedido.
            var detalle = await GetDetallePedidoAsync(pedidoId);

            if (detalle == null)
                return null;

            return new PagoFormDTO
            {
                Pedido = detalle,

                Pago = new PagoPedidoDTO
                {
                    PedidoId = pedido.PedidoId,
                    TotalPedido = pedido.Total
                },

                MetodosPago = metodosPago
                    .Where(m =>
                        m.Nombre.Equals("Tarjeta de crédito", StringComparison.OrdinalIgnoreCase) ||
                        m.Nombre.Equals("Tarjeta de débito", StringComparison.OrdinalIgnoreCase) ||
                        m.Nombre.Equals("Efectivo", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(m => m.MetodoPagoId)
                    .Select(m => new MetodoPagoDTO
                    {
                        MetodoPagoId = m.MetodoPagoId,
                        Nombre = m.Nombre
                    })
                    .ToList()
            };
        }

        public async Task ProcesarPagoAsync(PagoPedidoDTO dto, int usuarioActualId, string rolActual)
        {
            var pedido = await _repository.FindByIdAsync(dto.PedidoId);

            if (pedido == null)
            {
                throw new KeyNotFoundException("El pedido seleccionado no existe");
            }

            ValidarAccesoPedido(pedido, usuarioActualId, rolActual);

            if (!pedido.Estado.Nombre.Equals("Pendiente de pago", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("El pedido ya no se encuentra pendiente de pago");
            }

            if (pedido.MetodoPagoId.HasValue)
            {
                throw new InvalidOperationException("El pago de este pedido ya fue procesado");
            }

            if (!dto.MetodoPagoId.HasValue || dto.MetodoPagoId.Value <= 0)
            {
                throw new InvalidOperationException("Debe seleccionar un método de pago");
            }

            var metodoPago = await _repository.FindMetodoPagoByIdAsync(dto.MetodoPagoId.Value);

            if (metodoPago == null)
            {
                throw new InvalidOperationException("El método de pago seleccionado no existe");
            }

            bool esTarjeta = metodoPago.Nombre.Contains("Tarjeta", StringComparison.OrdinalIgnoreCase);

            bool esEfectivo = metodoPago.Nombre.Equals("Efectivo", StringComparison.OrdinalIgnoreCase);

            if (!esTarjeta && !esEfectivo)
            {
                throw new InvalidOperationException("El método de pago seleccionado no está permitido");
            }

            if (esTarjeta)
            {
                ProcesarPagoTarjeta(dto, pedido);
            }
            else
            {
                ProcesarPagoEfectivo(dto, pedido);
            }

            var estadoAceptada =await _repository.FindEstadoByNombreAsync("Aceptada");

            if (estadoAceptada == null)
            {
                throw new InvalidOperationException("No se encontró el estado Aceptada");
            }

            DateTime ahora = DateTime.Now;

            pedido.MetodoPagoId = metodoPago.MetodoPagoId;

            pedido.EstadoId = estadoAceptada.EstadoId;

            pedido.FechaActualizacion = ahora;

            pedido.PedidoHistorialEstado.Add(
                new PedidoHistorialEstado
                {
                    PedidoId = pedido.PedidoId,
                    EstadoId = estadoAceptada.EstadoId,
                    FechaCambio = ahora,
                    UsuarioId = usuarioActualId
                });

            await _repository.UpdateAsync(pedido);
        }
        #endregion

        #region Metodos para el pago en tarjeta
        //metodos helper de pago en tarjeta
        private static void ProcesarPagoTarjeta(PagoPedidoDTO dto, Pedido pedido)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreTitular))
            {
                throw new InvalidOperationException("Debe ingresar el nombre del titular");
            }

            if (string.IsNullOrWhiteSpace(dto.NumeroTarjeta))
            {
                throw new InvalidOperationException("Debe ingresar el número de tarjeta");
            }

            if (string.IsNullOrWhiteSpace(dto.FechaVencimiento))
            {
                throw new InvalidOperationException("Debe ingresar la fecha de vencimiento");
            }

            if (string.IsNullOrWhiteSpace(dto.CodigoSeguridad))
            {
                throw new InvalidOperationException("Debe ingresar el código de seguridad");
            }
            
            //verifica que la tarjeta no este vencida
            if (TarjetaVencida(dto.FechaVencimiento))
            {
                throw new InvalidOperationException("La tarjeta se encuentra vencida");
            }

            string numeroTarjeta = dto.NumeroTarjeta.Trim();

            if (numeroTarjeta.Length < 4)
            {
                throw new InvalidOperationException("El número de tarjeta no es válido");
            }

           //almacena los ultimos 4 digitos del cvv
            pedido.UltimosDigitosTarjeta =
                numeroTarjeta[^4..];

            pedido.MontoPagado =
                pedido.Total;

            pedido.Vuelto =
                0;
        }

        private static bool TarjetaVencida(string fechaVencimiento)
        {
            string[] partes = fechaVencimiento.Split('/');

            if (partes.Length != 2)
                return true;

            bool mesValido = int.TryParse(partes[0], out int mes);

            bool anioValido = int.TryParse(partes[1], out int anioCorto);

            if (!mesValido || !anioValido || mes < 1 || mes > 12)
            {
                return true;
            }

            int anioCompleto = 2000 + anioCorto;

            int ultimoDia = DateTime.DaysInMonth(anioCompleto, mes);

            DateTime fechaFinal = new DateTime(anioCompleto,mes, ultimoDia, 23, 59, 59);

            return fechaFinal < DateTime.Now;
        }
        #endregion

        #region Metodo para pago en Efectivo
        //metodo helper para pagp en efectivo
        private static void ProcesarPagoEfectivo(PagoPedidoDTO dto, Pedido pedido)
        {
            if (!dto.MontoPagado.HasValue)
            {
                throw new InvalidOperationException("Debe ingresar el monto recibido");
            }

            if (dto.MontoPagado.Value <= 0)
            {
                throw new InvalidOperationException("El monto recibido debe ser mayor a cero");
            }

            if (dto.MontoPagado.Value < pedido.Total)
            {
                throw new InvalidOperationException("El monto recibido no cubre el total del pedido");
            }

            pedido.MontoPagado = dto.MontoPagado.Value;

            pedido.Vuelto = Math.Round(dto.MontoPagado.Value - pedido.Total, 2, MidpointRounding.AwayFromZero);

            pedido.UltimosDigitosTarjeta = null;
        }

        #endregion

        #region Metodo para validar acceso al pedido segun el rol
        //helper para validar acceso al pedido
        private static void ValidarAccesoPedido(Pedido pedido, int usuarioActualId, string rolActual)
        {
            bool esAdministradorOEncargado =
                rolActual.Equals(
                    "Administrador",
                    StringComparison.OrdinalIgnoreCase)
                ||
                rolActual.Equals(
                    "Encargado",
                    StringComparison.OrdinalIgnoreCase);

            bool esPropietario = pedido.ClienteId == usuarioActualId;

            if (!esAdministradorOEncargado && !esPropietario)
            {
                throw new UnauthorizedAccessException("No tiene permiso para gestionar este pedido");
            }
        }
        #endregion

    }
}
