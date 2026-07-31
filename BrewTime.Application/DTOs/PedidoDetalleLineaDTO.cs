using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record PedidoDetalleLineaDTO
    {
        public string Producto { get; set; }

        public decimal Precio { get; set; }

        public int Cantidad { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public string Observaciones { get; set; }
    }
}
