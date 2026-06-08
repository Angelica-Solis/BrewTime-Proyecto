using BrewTime.Application.DTOs;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceCombo
    {
        Task<ICollection<ComboDTO>> ListAsync();
        Task<ComboDetalleDTO> FindByIdAsync(int id);
    }
}