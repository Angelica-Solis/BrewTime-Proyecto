using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.DTOs;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceProcesoPreparacion
    {
        Task<ICollection<ProcesoPreparacionDTO>> ListAsync();
        Task<ICollection<ProcesoPreparacionListadoDTO>>ListadoProcesosAsync();

        Task<ProcesoPreparacionDetalleDTO>DetailByProductoAsync(int productoId);
        Task<ProcesoPreparacionFormDTO> FindFormByIdAsync(int id);


        Task CreateAsync(ProcesoPreparacionFormDTO dto);
        Task UpdateAsync(ProcesoPreparacionFormDTO dto);
        // funciona solo para el edit, no se usa el mismo qeu create porque daña create
        Task<ProcesoPreparacionFormDTO> FindFormEditByProductoIdAsync(int productoId);
    }
}
