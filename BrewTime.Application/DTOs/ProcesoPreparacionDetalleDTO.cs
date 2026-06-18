using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record ProcesoPreparacionDetalleDTO
    {
        public string NombreProducto { get; set; }

        public ICollection<ProcesoPreparacionDTO> Procesos { get; set; }
    }
}
