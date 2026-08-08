using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryCocina
    {
        Task<IEnumerable<Pedido>> ObtenerPedidosAceptadosAsync();

        Task<bool> CambiarAEnPreparacionAsync(int pedidoId);

        Task<IEnumerable<Pedido>> ObtenerPedidosEnPreparacionAsync();

        Task<bool> MarcarPedidoEnCaminoAsync(int pedidoId);
    }
}
