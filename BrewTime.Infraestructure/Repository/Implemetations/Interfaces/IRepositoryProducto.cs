using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryProducto
    {
        Task<ICollection<Producto>> ListAsync();
        Task<ICollection<Producto>> ListInactivosAsync();
        Task<Producto> FindByIdAsync(int id);
        Task CreateAsync(Producto entity);
        Task UpdateAsync(Producto entity);
        void DeleteImagen(ProductoImagen imagen);
        Task ToggleActivoAsync(int id);
    }
}
