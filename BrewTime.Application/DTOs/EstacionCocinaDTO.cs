using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public class EstacionCocinaDTO
    {
        public int EstacionId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public int Orden { get; set; }

        public int TiempoEstimadoMin { get; set; }

        public string Estado { get; set; } = "Pendiente";

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }
    }
}
