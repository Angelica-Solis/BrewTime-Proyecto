using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class ValorPersonalizacion
{
    public int ValorId { get; set; }

    public int OpcionId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual OpcionPersonalizacion Opcion { get; set; } = null!;

    public virtual ICollection<PedidoDetalle> Detalle { get; set; } = new List<PedidoDetalle>();
}
