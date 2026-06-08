using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class MetodoPago
{
    public int MetodoPagoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();
}
