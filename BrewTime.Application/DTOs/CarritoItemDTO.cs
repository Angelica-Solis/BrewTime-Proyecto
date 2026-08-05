using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record CarritoItemDTO
    {
        public int CarritoId { get; set; }

        public int? ProductoId { get; set; }

        public int? ComboId { get; set; }
        public bool EsCombo => ComboId.HasValue;

        public string Nombre { get; set; } = "";

        public string? Imagen { get; set; }

        public decimal PrecioUnitario { get; set; }

        public int Cantidad { get; set; }

        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}
