using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryMenuCombo
    {
        Task<ICollection<MenuCombo>> ListAsync();
        Task<MenuCombo> FindByIdAsync(int menuId, int comboId);
    }
}
