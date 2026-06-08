using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class EstadoPedido
{
    public int EstadoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();

    public virtual ICollection<PedidoHistorialEstado> PedidoHistorialEstado { get; set; } = new List<PedidoHistorialEstado>();
}
