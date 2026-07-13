using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record EstacionProcesoDTO
    {
        public int EstacionId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public bool Seleccionada { get; set; }

        public int? Orden { get; set; }

        public int? TiempoEstimadoMin { get; set; }
    }

}

