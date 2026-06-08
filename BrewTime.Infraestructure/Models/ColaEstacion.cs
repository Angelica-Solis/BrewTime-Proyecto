using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class ColaEstacion
{
    public int ColaId { get; set; }

    public int DetalleId { get; set; }

    public int EstacionId { get; set; }

    public int Orden { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int? UsuarioCocinaId { get; set; }

    public virtual PedidoDetalle Detalle { get; set; } = null!;

    public virtual EstacionCocina Estacion { get; set; } = null!;

    public virtual Usuario? UsuarioCocina { get; set; }
}
