using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Infraestructure.Configuration
{
    public class OpenRouteServiceSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string DirectionsBaseUrl { get; set; } = string.Empty;
        public string GeocodingBaseUrl { get; set; } = string.Empty ;
        public double OrigenLatitud { get; set; }
        public double OrigenLongitud { get; set; }
    }
}
