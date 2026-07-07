using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public class IngredienteDTO
    {
        public int IngredienteID { get; set; }
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
    }
}
