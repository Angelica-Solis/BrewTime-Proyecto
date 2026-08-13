using BrewTime.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceRutaEntrega
    {
        Task<EntregaRutaDTO> CalcularRutaAsync(string direccion);
        Task<List<DireccionSugeridaDTO>> BuscarDireccionesAsync(string texto);
    }
}
