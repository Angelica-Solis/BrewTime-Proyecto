using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.DTOs;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceProducto
    {
        Task<ICollection<ProductoDTO>> ListAsync();
        Task<ICollection<ProductoDTO>> ListInactivosAsync();
        Task<ProductoDetalleDTO> FindByIdAsync(int id);
        Task<ProductoFormDTO> FindFormByIdAsync(int id);
        Task CreateAsync(ProductoFormDTO dto, string wwwRootPath);
        Task UpdateAsync(ProductoFormDTO dto, string wwwRootPath);
        Task ToggleActivoAsync(int id);
    }
}
