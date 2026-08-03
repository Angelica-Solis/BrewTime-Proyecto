using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceFaqKnowledgeBase
    {
        // Devuelve el texto completo del PDF de preguntas frecuentes.
        // El resultado se cachea en memoria, por lo que el PDF solo se
        // lee una vez durante la vida de la aplicacion.
        Task<string> ObtenerContenidoAsync();
    }
}
