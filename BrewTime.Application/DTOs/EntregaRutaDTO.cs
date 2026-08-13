using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public class EntregaRutaDTO
    {
        //obtiene la direccion del usuario
        public string DireccionEncontrada { get; set; } = string.Empty;

        public double LatitudDestino { get; set; }

        public double LongitudDestino { get; set; }

        public double DistanciaKilometro { get; set; }

        public int TiempoEstimado { get; set; }

        public decimal CostoPorDistancia { get; set; } //costo que se suma al costo base del envio
    }
}
