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

                Encargado = pedido.Empleado != null
                    ? $"{pedido.Empleado.Nombre} {pedido.Empleado.Apellidos}"
                    : "Sin asignar",

                MetodoEntrega = pedido.MetodoEntrega.Nombre,

                MetodoPago = pedido.MetodoPago != null
                    ? pedido.MetodoPago.Nombre
                    : "No registrado",

                Estado = pedido.Estado.Nombre,

                Subtotal = pedido.Subtotal,

                Impuesto = pedido.Impuesto,

                CostoEnvio = pedido.CostoEnvio,

                Total = pedido.Total,

                Detalles = new List<PedidoDetalleLineaDTO>()
            };

            foreach (var item in pedido.PedidoDetalle)
            {
                detalle.Detalles.Add(new PedidoDetalleLineaDTO
                {
                    Producto = item.Producto != null
                        ? item.Producto.Nombre
                        : item.Combo!.Nombre,

                    Precio = item.PrecioUnitario,

                    Cantidad = item.Cantidad,

                    Subtotal = item.Subtotal,

                    Impuesto = pedido.Subtotal > 0
                    ? Math.Round((item.Subtotal / pedido.Subtotal) * pedido.Impuesto, 2)
                    : 0,

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
                //el encargado debe seleccionar al cliente.
                if (!dto.ClienteId.HasValue || dto.ClienteId.Value <= 0)
                {
                    throw new InvalidOperationException("Debe seleccionar el cliente del pedido");
                }

                var clienteSeleccionado = await _repositoryUsuario.FindByIdAsync(dto.ClienteId.Value);

                bool esClienteValido =
                    clienteSeleccionado != null &&
                    clienteSeleccionado.Activo &&
                    clienteSeleccionado.Rol != null &&
                    clienteSeleccionado.Rol.Nombre.Equals(
                        "Cliente",
                        StringComparison.OrdinalIgnoreCase);

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
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Last().Observaciones);

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

                        UsuarioId =
                            usuarioActualId
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
    }
}
