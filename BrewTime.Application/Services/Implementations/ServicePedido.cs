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
            if (pedido == null) return null;
            return _mapper.Map<PedidoDetalleDTO>(pedido);
        }

        public async Task<ICollection<EstadoPedidoDTO>> GetEstadosAsync()
        {
            var estados = await _repository.ListEstadosAsync();
            return _mapper.Map<ICollection<EstadoPedidoDTO>>(estados);
        }
    }
}
