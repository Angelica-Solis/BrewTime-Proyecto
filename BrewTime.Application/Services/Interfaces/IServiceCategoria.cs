using BrewTime.Application.DTOs;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceCategoria
    {
        Task<ICollection<CategoriaDTO>> ListActivasAsync();
    }
}
