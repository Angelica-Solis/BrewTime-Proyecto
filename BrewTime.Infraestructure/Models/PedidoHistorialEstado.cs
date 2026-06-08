using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class PedidoHistorialEstado
{
    public int HistorialId { get; set; }

    public int PedidoId { get; set; }

    public int EstadoId { get; set; }

    public DateTime FechaCambio { get; set; }

    public int? UsuarioId { get; set; }

    public virtual EstadoPedido Estado { get; set; } = null!;

    public virtual Pedido Pedido { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
