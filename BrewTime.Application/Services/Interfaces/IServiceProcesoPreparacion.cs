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
        Task<ProcesoPreparacionDTO> FindByIdAsync(int id);


    }
}
