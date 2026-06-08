using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class ComboProducto
{
    public int ComboId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public virtual Combo Combo { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;
}
