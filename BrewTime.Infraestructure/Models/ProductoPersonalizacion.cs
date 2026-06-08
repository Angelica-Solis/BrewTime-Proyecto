using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class ProductoPersonalizacion
{
    public int ProductoId { get; set; }

    public int OpcionId { get; set; }

    public bool Requerida { get; set; }

    public virtual OpcionPersonalizacion Opcion { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;
}
