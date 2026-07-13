using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryMenu
    {
        Task<ICollection<Menu>> ListAsync();

        Task<Menu?> FindByIdAsync(int id);

        Task<Menu?> FindDisponibleAsync(DateOnly fechaActual,TimeOnly horaActual);

        Task CreateAsync(Menu entity,ICollection<int> productoIds,ICollection<int> comboIds, ICollection<byte> dias);

        Task UpdateAsync(Menu entity,ICollection<int> productoIds,ICollection<int> comboIds, ICollection<byte> dias);

        Task ToggleActivoAsync(int id);
        Task<bool> ExisteNombreAsync(string nombre,int? menuIdExcluir = null);
    }
}
