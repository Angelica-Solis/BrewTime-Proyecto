using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceCorreo
    {
        Task EnviarAsync(string destino, string asunto, string cuerpoHtml);

    }
}
