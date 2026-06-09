using BrewTime.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceMenu
    {
        Task<ICollection<MenuDTO>> ListAsync();
        Task<MenuDTO> FindByIdAsync(int id);
    }
}
