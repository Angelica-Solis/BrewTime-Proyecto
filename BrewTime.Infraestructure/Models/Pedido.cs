using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class Pedido
{
    public int PedidoId { get; set; }

    public int ClienteId { get; set; }

    public int? EmpleadoId { get; set; }

    public int EstadoId { get; set; }

    public int MetodoEntregaId { get; set; }

    public int? MetodoPagoId { get; set; }

    public string? DireccionEntrega { get; set; }

    public decimal CostoEnvio { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Total { get; set; }

    public decimal? MontoPagado { get; set; }

    public decimal? Vuelto { get; set; }

    public string? UltimosDigitosTarjeta { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public virtual Usuario Cliente { get; set; } = null!;

    public virtual Usuario? Empleado { get; set; }

    public virtual EstadoPedido Estado { get; set; } = null!;

    public virtual MetodoEntrega MetodoEntrega { get; set; } = null!;

    public virtual MetodoPago? MetodoPago { get; set; }

    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();

    public virtual ICollection<PedidoHistorialEstado> PedidoHistorialEstado { get; set; } = new List<PedidoHistorialEstado>();
}
