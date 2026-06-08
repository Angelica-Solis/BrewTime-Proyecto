using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class MetodoEntrega
{
    public int MetodoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();
}
