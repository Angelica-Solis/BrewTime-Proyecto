using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class Ingrediente
{
    public int IngredienteId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();
}
