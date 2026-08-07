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

        //nombre de los estados del pedido
        Task<EstadoPedido?> FindEstadoByNombreAsync(string nombre);

        //metodos de entrega
        Task<ICollection<MetodoEntrega>>ListMetodosEntregaAsync();

        Task<MetodoEntrega?> FindMetodoEntregaByIdAsync(int metodoEntregaId);

        //metodo de pago
        Task<ICollection<MetodoPago>>ListMetodosPagoAsync();

        Task<MetodoPago?> FindMetodoPagoByIdAsync(int metodoPagoId);

        //registrar y actualizar pedidos
        Task CreateAsync(Pedido pedido);

        Task UpdateAsync(Pedido pedido);
    }
}
