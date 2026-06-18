using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record EstacionProcesoDTO
    {
        public string NombreEstacion { get; set; }

        public int Orden { get; set; }
    }
}
