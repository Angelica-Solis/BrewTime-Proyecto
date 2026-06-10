using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Application.DTOs
{
    public record ProcesoPreparacionDTO
    {
        public int ProcesoId { get; set; }

        public Producto Producto { get; set; }

        public EstacionCocina Estacion { get; set; }

        public int Orden { get; set; }

        public int TiempoEstimadoMin { get; set; }
    }
}
