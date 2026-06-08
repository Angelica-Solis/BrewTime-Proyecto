using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class Categoria
{
    public int CategoriaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Combo> Combo { get; set; } = new List<Combo>();

    public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();
}
