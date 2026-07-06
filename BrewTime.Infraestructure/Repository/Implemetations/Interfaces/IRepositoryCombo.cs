using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryCombo
    {
        Task<ICollection<Combo>> ListAsync();
        Task<ICollection<Combo>> ListInactivosAsync();
        Task<Combo> FindByIdAsync(int id);
        Task CreateAsync(Combo entity);
        Task UpdateAsync(Combo entity);
        Task ToggleActivoAsync(int id);
    }
}