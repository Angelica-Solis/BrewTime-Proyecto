using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.DTOs;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceEstacionCocina
    {
        Task<ICollection<EstacionProcesoDTO>> ListAsync();
    }
}
