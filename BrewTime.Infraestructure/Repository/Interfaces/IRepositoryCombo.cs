using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryCombo
    {
        Task<ICollection<Combo>> ListAsync();
        Task<Combo> FindByIdAsync(int id);
    }
}