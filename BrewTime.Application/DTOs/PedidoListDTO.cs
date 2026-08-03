using System;

namespace BrewTime.Application.DTOs
{
    public class PedidoListDTO
    {
        public int PedidoId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string ClienteNombre { get; set; } = null!;
        public string EstadoNombre { get; set; } = null!;
        public decimal Total { get; set; }
        public string MetodoEntrega { get; set; } = null!;
    }
}
