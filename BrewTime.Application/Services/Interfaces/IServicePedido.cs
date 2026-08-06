using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrewTime.Application.DTOs;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServicePedido
    {
        Task<ICollection<PedidoListDTO>> GetHistorialClienteAsync(int clienteId);
        Task<ICollection<PedidoListDTO>> GetTodosPedidosAsync(DateTime? fecha, int? estadoId);
        Task<PedidoDetalleDTO?> GetDetallePedidoAsync(int pedidoId);
        Task<ICollection<EstadoPedidoDTO>> GetEstadosAsync();

        Task<PedidoCreateDTO>PrepararRegistroAsync(int usuarioActualId, string rolActual);
        Task<int>RegistrarDesdeCarritoAsync(PedidoCreateDTO dto, int usuarioActualId, string rolActual);
    }
}
