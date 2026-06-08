using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class Rol
{
    public int RolId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
