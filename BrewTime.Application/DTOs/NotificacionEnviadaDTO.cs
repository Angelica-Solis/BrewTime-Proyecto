using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record NotificacionEnviadaDTO
    {
        public DateTime FechaEnvio { get; set; }
        public string Asunto { get; set; } = null!;
        public string Detalle { get; set; } = null!;
    }
}
