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

        Task<MenuDTO?> FindByIdAsync(int id);

        Task<MenuFormDTO?> FindFormByIdAsync(int id);

        Task<MenuDTO?> FindDisponibleAsync(DateOnly fechaActual,TimeOnly horaActual);

        Task CreateAsync(MenuFormDTO dto);

        Task UpdateAsync(MenuFormDTO dto);

        Task ToggleActivoAsync(int id);

        Task<bool> ExisteNombreAsync(string nombre,int? menuIdExcluir = null);
    }
}
