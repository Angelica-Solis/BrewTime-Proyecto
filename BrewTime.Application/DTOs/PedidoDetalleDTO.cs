using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record  PedidoDetalleDTO
    {
        public int PedidoId { get; set; }

        public DateTime Fecha { get; set; }

        public string ClienteNombre { get; set; }

        public string ClienteCorreo { get; set; }

        public string Encargado { get; set; }

        public string MetodoEntrega { get; set; }

        public string MetodoPago { get; set; }

        public string Estado { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public List<PedidoDetalleLineaDTO> Detalles { get; set; }
    }
}
