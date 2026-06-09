using BrewTime.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceMenuCombo
    {
        Task<ICollection<MenuComboDTO>> ListAsync();
        Task<MenuComboDTO> FindByIdAsync(int menuId, int comboId);
    }
}
