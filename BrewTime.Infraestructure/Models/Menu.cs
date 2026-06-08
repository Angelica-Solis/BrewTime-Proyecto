using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class Menu
{
    public int MenuId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<MenuDiaSemana> MenuDiaSemana { get; set; } = new List<MenuDiaSemana>();

    public virtual ICollection<Combo> Combo { get; set; } = new List<Combo>();

    public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();
}
