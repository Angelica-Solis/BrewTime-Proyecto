using BrewTime.Application.DTOs;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceCombo
    {
        Task<ICollection<ComboDTO>> ListAsync();
        Task<ICollection<ComboDTO>> ListInactivosAsync();
        Task<ComboDetalleDTO> FindByIdAsync(int id);
        Task<ComboFormDTO> FindFormByIdAsync(int id);
        Task CreateAsync(ComboFormDTO dto, string wwwRootPath);
        Task UpdateAsync(ComboFormDTO dto, string wwwRootPath);
        Task ToggleActivoAsync(int id);

        //para la tarea programada 
        Task<ICollection<ProductoInconsistenteEnComboDTO>> ObtenerProductosInconsistentesAsync();
        Task<bool> RevisarYNotificarProductosInconsistentesAsync();
    }
}