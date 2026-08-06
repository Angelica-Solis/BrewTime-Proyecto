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

        public int ClienteId { get; set; }

        public string ClienteNombre { get; set; } = string.Empty;

        public string ClienteCorreo { get; set; } = string.Empty;

        public string Encargado { get; set; } = string.Empty;

        public string MetodoEntrega { get; set; } = string.Empty;

        public string? DireccionEntrega { get; set; }

        public string MetodoPago { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }

        public decimal Impuesto { get; set; }

        public decimal CostoEnvio { get; set; }

        public decimal Total { get; set; }

        public decimal? MontoPagado { get; set; }

        public decimal? Vuelto { get; set; }

        public string? UltimosDigitosTarjeta { get; set; }

        public List<PedidoDetalleLineaDTO> Detalles { get; set; } = new();
    }
}
