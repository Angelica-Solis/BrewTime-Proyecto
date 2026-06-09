using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class Producto
{
    public int ProductoId { get; set; }

    public int CategoriaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual ICollection<ComboProducto> ComboProducto { get; set; } = new List<ComboProducto>();

    public virtual ICollection<HistorialConsumo> HistorialConsumo { get; set; } = new List<HistorialConsumo>();

    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();

    public virtual ICollection<ProcesoPreparacion> ProcesoPreparacion { get; set; } = new List<ProcesoPreparacion>();

    public virtual ICollection<ProductoImagen> ProductoImagen { get; set; } = new List<ProductoImagen>();

    public virtual ICollection<ProductoPersonalizacion> ProductoPersonalizacion { get; set; } = new List<ProductoPersonalizacion>();

    public virtual ICollection<Ingrediente> Ingrediente { get; set; } = new List<Ingrediente>();

    public virtual ICollection<MenuProducto> MenuProducto { get; set; } = new List<MenuProducto>();
}
