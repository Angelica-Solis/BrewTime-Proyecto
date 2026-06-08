using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class PedidoDetalle
{
    public int DetalleId { get; set; }

    public int PedidoId { get; set; }

    public int? ProductoId { get; set; }

    public int? ComboId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public string? Observaciones { get; set; }

    public virtual ICollection<ColaEstacion> ColaEstacion { get; set; } = new List<ColaEstacion>();

    public virtual Combo? Combo { get; set; }

    public virtual Pedido Pedido { get; set; } = null!;

    public virtual Producto? Producto { get; set; }

    public virtual ICollection<ValorPersonalizacion> Valor { get; set; } = new List<ValorPersonalizacion>();
}
