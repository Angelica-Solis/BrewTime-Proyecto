using System.Collections.Generic;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryPedido
    {
        Task<ICollection<Pedido>> ListAllAsync();
        Task<ICollection<Pedido>> ListByClienteAsync(int clienteId);
        Task<Pedido?> FindByIdAsync(int id);
        Task<ICollection<EstadoPedido>> ListEstadosAsync();
    }
}
