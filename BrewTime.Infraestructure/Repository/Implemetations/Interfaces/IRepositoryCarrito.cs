using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryCarrito
    {
        Task<ICollection<Carrito>> GetByUsuarioAsync(int usuarioId);

        Task<Carrito?> GetProductoAsync(int usuarioId, int productoId);

        Task<Carrito?> GetComboAsync(int usuarioId, int comboId);

        Task AddAsync(Carrito carrito);

        Task UpdateAsync(Carrito carrito);

        Task DeleteAsync(int carritoId);

        Task DeleteAllAsync(int usuarioId);

        Task SaveChangesAsync();
        Task<Carrito?> FindByIdAsync(int carritoId);

        Task<int> CantidadItemsAsync(int usuarioId);
    }
}
