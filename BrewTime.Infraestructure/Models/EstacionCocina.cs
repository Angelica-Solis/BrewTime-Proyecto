using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class EstacionCocina
{
    public int EstacionId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<ColaEstacion> ColaEstacion { get; set; } = new List<ColaEstacion>();

    public virtual ICollection<ProcesoPreparacion> ProcesoPreparacion { get; set; } = new List<ProcesoPreparacion>();
}
