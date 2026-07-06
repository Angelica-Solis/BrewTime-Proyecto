using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryCategoria
    {
        Task<ICollection<Categoria>> ListActivasAsync();
    }
}
