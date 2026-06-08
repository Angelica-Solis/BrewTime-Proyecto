using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class ProcesoPreparacion
{
    public int ProcesoId { get; set; }

    public int ProductoId { get; set; }

    public int EstacionId { get; set; }

    public int Orden { get; set; }

    public int TiempoEstimadoMin { get; set; }

    public virtual EstacionCocina Estacion { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;
}
