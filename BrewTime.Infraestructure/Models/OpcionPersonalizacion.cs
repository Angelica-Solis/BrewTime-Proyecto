using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class OpcionPersonalizacion
{
    public int OpcionId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<ProductoPersonalizacion> ProductoPersonalizacion { get; set; } = new List<ProductoPersonalizacion>();

    public virtual ICollection<ValorPersonalizacion> ValorPersonalizacion { get; set; } = new List<ValorPersonalizacion>();
}
