using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record ProcesoPreparacionListadoDTO
    {
        public int ProductoId { get; set; }

        public string NombreProducto { get; set; }

        public int CantidadProcesos { get; set; }
    }
}

