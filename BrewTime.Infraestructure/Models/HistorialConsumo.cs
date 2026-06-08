using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class HistorialConsumo
{
    public int HistorialId { get; set; }

    public int UsuarioId { get; set; }

    public int ProductoId { get; set; }

    public string Origen { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
