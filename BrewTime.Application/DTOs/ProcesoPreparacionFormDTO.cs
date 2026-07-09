using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Application.DTOs
{
    public class ProcesoPreparacionFormDTO
    {
        public int ProductoId { get; set; }

        public List<EstacionProcesoDTO> Estaciones { get; set; } = new();
    }
}
