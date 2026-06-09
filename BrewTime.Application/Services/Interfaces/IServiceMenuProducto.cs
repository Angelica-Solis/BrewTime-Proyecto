using BrewTime.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceMenuProducto
    {
        Task<ICollection<MenuProductoDTO>> ListAsync();
        Task<MenuProductoDTO> FindByIdAsync(int menuId, int productoId);
    }
}
