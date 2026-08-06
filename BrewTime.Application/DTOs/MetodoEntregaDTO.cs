using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public class MetodoEntregaDTO
    {
        public int MetodoId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public decimal Costo { get; set; }
    }
}
