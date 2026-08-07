using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;

namespace BrewTime.Application.Services.Implementations
{
    public class ServicePedido : IServicePedido
    {
        private readonly IRepositoryPedido _repository;
        private readonly IMapper _mapper;

        public ServicePedido(IRepositoryPedido repository, IMapper mapper)
        {
            _repository = repository;
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

                CostoEnvio = pedido.MetodoEntrega.Costo,

                Total = pedido.Subtotal
                  + pedido.Impuesto
                  + pedido.MetodoEntrega.Costo,

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
    }
}
