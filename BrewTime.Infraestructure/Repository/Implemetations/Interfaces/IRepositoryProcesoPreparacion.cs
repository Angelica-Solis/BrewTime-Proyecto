using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryProcesoPreparacion
    {
        Task<ICollection<ProcesoPreparacion>> ListAsync();
        Task<ICollection<ProcesoPreparacion>> FindByProductoIdAsync(int productoId);
        Task<ProcesoPreparacion> FindByIdAsync(int id);


        Task CreateAsync(ProcesoPreparacion entity);
        Task CreateRangeAsync(IEnumerable<ProcesoPreparacion> entities);
        Task UpdateAsync(ProcesoPreparacion entity);

        Task DeleteByProductoIdAsync(int productoId);

    }
}
