using BrewTime.Application.DTOs;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceCombo
    {
        Task<ICollection<ComboDTO>> ListAsync();
        Task<ICollection<ComboDTO>> ListInactivosAsync();
        Task<ComboDetalleDTO> FindByIdAsync(int id);
        Task<ComboFormDTO> FindFormByIdAsync(int id);
        Task CreateAsync(ComboFormDTO dto);
        Task UpdateAsync(ComboFormDTO dto);
        Task ToggleActivoAsync(int id);
    }
}