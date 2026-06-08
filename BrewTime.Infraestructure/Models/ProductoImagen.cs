using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class ProductoImagen
{
    public int ImagenId { get; set; }

    public int ProductoId { get; set; }

    public string RutaImagen { get; set; } = null!;

    public bool EsPrincipal { get; set; }

    public DateTime FechaSubida { get; set; }

    public virtual Producto Producto { get; set; } = null!;
}
