using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record AgregarCarritoDTO
    {
        public int? ProductoId { get; set; }

        public int? ComboId { get; set; }

        public int Cantidad { get; set; } = 1;
    }
}
