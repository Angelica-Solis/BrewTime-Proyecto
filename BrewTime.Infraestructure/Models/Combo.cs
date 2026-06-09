using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class Combo
{
    public int ComboId { get; set; }

    public int CategoriaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal PrecioEspecial { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual ICollection<ComboProducto> ComboProducto { get; set; } = new List<ComboProducto>();

    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();

    public virtual ICollection<MenuCombo> MenuCombo { get; set; } = new List<MenuCombo>();
}
