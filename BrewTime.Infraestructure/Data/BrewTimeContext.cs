using System;
using System.Collections.Generic;
using BrewTime.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BrewTime.Infraestructure.Data;

public partial class BrewTimeContext : DbContext
{
    public BrewTimeContext(DbContextOptions<BrewTimeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<ColaEstacion> ColaEstacion { get; set; }

    public virtual DbSet<Combo> Combo { get; set; }

    public virtual DbSet<ComboProducto> ComboProducto { get; set; }

    public virtual DbSet<EstacionCocina> EstacionCocina { get; set; }

    public virtual DbSet<EstadoPedido> EstadoPedido { get; set; }

    public virtual DbSet<HistorialConsumo> HistorialConsumo { get; set; }

    public virtual DbSet<Ingrediente> Ingrediente { get; set; }

    public virtual DbSet<Menu> Menu { get; set; }

    public virtual DbSet<MenuDiaSemana> MenuDiaSemana { get; set; }

    public virtual DbSet<MetodoEntrega> MetodoEntrega { get; set; }

    public virtual DbSet<MetodoPago> MetodoPago { get; set; }

    public virtual DbSet<OpcionPersonalizacion> OpcionPersonalizacion { get; set; }

    public virtual DbSet<Pedido> Pedido { get; set; }

    public virtual DbSet<PedidoDetalle> PedidoDetalle { get; set; }

    public virtual DbSet<PedidoHistorialEstado> PedidoHistorialEstado { get; set; }

    public virtual DbSet<ProcesoPreparacion> ProcesoPreparacion { get; set; }

    public virtual DbSet<Producto> Producto { get; set; }

    public virtual DbSet<ProductoImagen> ProductoImagen { get; set; }

    public virtual DbSet<ProductoPersonalizacion> ProductoPersonalizacion { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    public virtual DbSet<ValorPersonalizacion> ValorPersonalizacion { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1C51122AC6E");

            entity.HasIndex(e => e.Nombre, "UQ__Categori__75E3EFCF92FDF04E").IsUnique();

            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ColaEstacion>(entity =>
        {
            entity.HasKey(e => e.ColaId).HasName("PK__ColaEsta__B00C223B90FCBFCF");

            entity.Property(e => e.ColaId).HasColumnName("ColaID");
            entity.Property(e => e.DetalleId).HasColumnName("DetalleID");
            entity.Property(e => e.EstacionId).HasColumnName("EstacionID");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("En cola");
            entity.Property(e => e.FechaFin).HasColumnType("datetime");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime");
            entity.Property(e => e.UsuarioCocinaId).HasColumnName("UsuarioCocinaID");

            entity.HasOne(d => d.Detalle).WithMany(p => p.ColaEstacion)
                .HasForeignKey(d => d.DetalleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ColaEstac__Detal__17036CC0");

            entity.HasOne(d => d.Estacion).WithMany(p => p.ColaEstacion)
                .HasForeignKey(d => d.EstacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ColaEstac__Estac__17F790F9");

            entity.HasOne(d => d.UsuarioCocina).WithMany(p => p.ColaEstacion)
                .HasForeignKey(d => d.UsuarioCocinaId)
                .HasConstraintName("FK__ColaEstac__Usuar__19DFD96B");
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.ComboId).HasName("PK__Combo__DD42580ED5CA08AB");

            entity.HasIndex(e => e.Nombre, "UQ__Combo__75E3EFCF03017B02").IsUnique();

            entity.Property(e => e.ComboId).HasColumnName("ComboID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PrecioEspecial).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Combo)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Combo__Categoria__5070F446");
        });

        modelBuilder.Entity<ComboProducto>(entity =>
        {
            entity.HasKey(e => new { e.ComboId, e.ProductoId }).HasName("PK__ComboPro__F70152E68DB7E627");

            entity.Property(e => e.ComboId).HasColumnName("ComboID");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.Cantidad).HasDefaultValue(1);

            entity.HasOne(d => d.Combo).WithMany(p => p.ComboProducto)
                .HasForeignKey(d => d.ComboId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ComboProd__Combo__5535A963");

            entity.HasOne(d => d.Producto).WithMany(p => p.ComboProducto)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ComboProd__Produ__5629CD9C");
        });

        modelBuilder.Entity<EstacionCocina>(entity =>
        {
            entity.HasKey(e => e.EstacionId).HasName("PK__Estacion__D9998F3F30F0BCB7");

            entity.HasIndex(e => e.Nombre, "UQ__Estacion__75E3EFCFC090FCC5").IsUnique();

            entity.Property(e => e.EstacionId).HasColumnName("EstacionID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoPedido>(entity =>
        {
            entity.HasKey(e => e.EstadoId).HasName("PK__EstadoPe__FEF86B6009A731D8");

            entity.HasIndex(e => e.Nombre, "UQ__EstadoPe__75E3EFCFA0EE158F").IsUnique();

            entity.Property(e => e.EstadoId).HasColumnName("EstadoID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HistorialConsumo>(entity =>
        {
            entity.HasKey(e => e.HistorialId).HasName("PK__Historia__975206EFC01CD1D7");

            entity.Property(e => e.HistorialId).HasColumnName("HistorialID");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Origen)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pedido");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Producto).WithMany(p => p.HistorialConsumo)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Produ__2B0A656D");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialConsumo)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Usuar__2A164134");
        });

        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.IngredienteId).HasName("PK__Ingredie__CCB95EC8A08EF5BB");

            entity.HasIndex(e => e.Nombre, "UQ__Ingredie__75E3EFCFCE1B0D58").IsUnique();

            entity.Property(e => e.IngredienteId).HasColumnName("IngredienteID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Menu__C99ED250538CAC98");

            entity.HasIndex(e => e.Nombre, "UQ__Menu__75E3EFCF98FB7C52").IsUnique();

            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasMany(d => d.Combo).WithMany(p => p.Menu)
                .UsingEntity<Dictionary<string, object>>(
                    "MenuCombo",
                    r => r.HasOne<Combo>().WithMany()
                        .HasForeignKey("ComboId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MenuCombo__Combo__66603565"),
                    l => l.HasOne<Menu>().WithMany()
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MenuCombo__MenuI__656C112C"),
                    j =>
                    {
                        j.HasKey("MenuId", "ComboId").HasName("PK__MenuComb__344AF7D050C1CBC9");
                        j.IndexerProperty<int>("MenuId").HasColumnName("MenuID");
                        j.IndexerProperty<int>("ComboId").HasColumnName("ComboID");
                    });

            entity.HasMany(d => d.Producto).WithMany(p => p.Menu)
                .UsingEntity<Dictionary<string, object>>(
                    "MenuProducto",
                    r => r.HasOne<Producto>().WithMany()
                        .HasForeignKey("ProductoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MenuProdu__Produ__628FA481"),
                    l => l.HasOne<Menu>().WithMany()
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MenuProdu__MenuI__619B8048"),
                    j =>
                    {
                        j.HasKey("MenuId", "ProductoId").HasName("PK__MenuProd__E3DDD8B8887A4DB9");
                        j.IndexerProperty<int>("MenuId").HasColumnName("MenuID");
                        j.IndexerProperty<int>("ProductoId").HasColumnName("ProductoID");
                    });
        });

        modelBuilder.Entity<MenuDiaSemana>(entity =>
        {
            entity.HasKey(e => new { e.MenuId, e.DiaSemana }).HasName("PK__MenuDiaS__7E92D1A44430BD45");

            entity.Property(e => e.MenuId).HasColumnName("MenuID");

            entity.HasOne(d => d.Menu).WithMany(p => p.MenuDiaSemana)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MenuDiaSe__MenuI__5EBF139D");
        });

        modelBuilder.Entity<MetodoEntrega>(entity =>
        {
            entity.HasKey(e => e.MetodoId).HasName("PK__MetodoEn__5C1E3E31E4B2D7B7");

            entity.HasIndex(e => e.Nombre, "UQ__MetodoEn__75E3EFCF557426BF").IsUnique();

            entity.Property(e => e.MetodoId).HasColumnName("MetodoID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.MetodoPagoId).HasName("PK__MetodoPa__A8FEAF742F9B20AA");

            entity.HasIndex(e => e.Nombre, "UQ__MetodoPa__75E3EFCFD912E74A").IsUnique();

            entity.Property(e => e.MetodoPagoId).HasColumnName("MetodoPagoID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OpcionPersonalizacion>(entity =>
        {
            entity.HasKey(e => e.OpcionId).HasName("PK__OpcionPe__77CD080300F80553");

            entity.Property(e => e.OpcionId).HasColumnName("OpcionID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.PedidoId).HasName("PK__Pedido__09BA14101125D7F6");

            entity.Property(e => e.PedidoId).HasColumnName("PedidoID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.CostoEnvio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DireccionEntrega)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.EstadoId).HasColumnName("EstadoID");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MetodoEntregaId).HasColumnName("MetodoEntregaID");
            entity.Property(e => e.MetodoPagoId).HasColumnName("MetodoPagoID");
            entity.Property(e => e.MontoPagado).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UltimosDigitosTarjeta)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Vuelto).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Cliente).WithMany(p => p.PedidoCliente)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pedido__ClienteI__7B5B524B");

            entity.HasOne(d => d.Empleado).WithMany(p => p.PedidoEmpleado)
                .HasForeignKey(d => d.EmpleadoId)
                .HasConstraintName("FK__Pedido__Empleado__7C4F7684");

            entity.HasOne(d => d.Estado).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pedido__EstadoID__7D439ABD");

            entity.HasOne(d => d.MetodoEntrega).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.MetodoEntregaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pedido__MetodoEn__7E37BEF6");

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.MetodoPagoId)
                .HasConstraintName("FK__Pedido__MetodoPa__7F2BE32F");
        });

        modelBuilder.Entity<PedidoDetalle>(entity =>
        {
            entity.HasKey(e => e.DetalleId).HasName("PK__PedidoDe__6E19D6FAD685104E");

            entity.Property(e => e.DetalleId).HasColumnName("DetalleID");
            entity.Property(e => e.Cantidad).HasDefaultValue(1);
            entity.Property(e => e.ComboId).HasColumnName("ComboID");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.PedidoId).HasColumnName("PedidoID");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Combo).WithMany(p => p.PedidoDetalle)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("FK__PedidoDet__Combo__0F624AF8");

            entity.HasOne(d => d.Pedido).WithMany(p => p.PedidoDetalle)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidoDet__Pedid__0D7A0286");

            entity.HasOne(d => d.Producto).WithMany(p => p.PedidoDetalle)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__PedidoDet__Produ__0E6E26BF");

            entity.HasMany(d => d.Valor).WithMany(p => p.Detalle)
                .UsingEntity<Dictionary<string, object>>(
                    "PedidoDetallePersonalizacion",
                    r => r.HasOne<ValorPersonalizacion>().WithMany()
                        .HasForeignKey("ValorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PedidoDet__Valor__14270015"),
                    l => l.HasOne<PedidoDetalle>().WithMany()
                        .HasForeignKey("DetalleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PedidoDet__Detal__1332DBDC"),
                    j =>
                    {
                        j.HasKey("DetalleId", "ValorId").HasName("PK__PedidoDe__3BC401E42D80FAC9");
                        j.IndexerProperty<int>("DetalleId").HasColumnName("DetalleID");
                        j.IndexerProperty<int>("ValorId").HasColumnName("ValorID");
                    });
        });

        modelBuilder.Entity<PedidoHistorialEstado>(entity =>
        {
            entity.HasKey(e => e.HistorialId).HasName("PK__PedidoHi__975206EF17B176A7");

            entity.Property(e => e.HistorialId).HasColumnName("HistorialID");
            entity.Property(e => e.EstadoId).HasColumnName("EstadoID");
            entity.Property(e => e.FechaCambio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PedidoId).HasColumnName("PedidoID");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Estado).WithMany(p => p.PedidoHistorialEstado)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidoHis__Estad__08B54D69");

            entity.HasOne(d => d.Pedido).WithMany(p => p.PedidoHistorialEstado)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidoHis__Pedid__07C12930");

            entity.HasOne(d => d.Usuario).WithMany(p => p.PedidoHistorialEstado)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__PedidoHis__Usuar__0A9D95DB");
        });

        modelBuilder.Entity<ProcesoPreparacion>(entity =>
        {
            entity.HasKey(e => e.ProcesoId).HasName("PK__ProcesoP__1C00FFF0E72A0BCE");

            entity.HasIndex(e => new { e.ProductoId, e.Orden }, "UQ__ProcesoP__A24A93044495DD4C").IsUnique();

            entity.Property(e => e.ProcesoId).HasColumnName("ProcesoID");
            entity.Property(e => e.EstacionId).HasColumnName("EstacionID");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.TiempoEstimadoMin).HasDefaultValue(5);

            entity.HasOne(d => d.Estacion).WithMany(p => p.ProcesoPreparacion)
                .HasForeignKey(d => d.EstacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProcesoPr__Estac__6EF57B66");

            entity.HasOne(d => d.Producto).WithMany(p => p.ProcesoPreparacion)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProcesoPr__Produ__6E01572D");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__A430AE836219F41F");

            entity.HasIndex(e => e.Nombre, "UQ__Producto__75E3EFCF0D933EDD").IsUnique();

            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Producto)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Producto__Catego__36B12243");

            entity.HasMany(d => d.Ingrediente).WithMany(p => p.Producto)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductoIngrediente",
                    r => r.HasOne<Ingrediente>().WithMany()
                        .HasForeignKey("IngredienteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProductoI__Ingre__3C69FB99"),
                    l => l.HasOne<Producto>().WithMany()
                        .HasForeignKey("ProductoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProductoI__Produ__3B75D760"),
                    j =>
                    {
                        j.HasKey("ProductoId", "IngredienteId").HasName("PK__Producto__38FB3B6FE326D6C7");
                        j.IndexerProperty<int>("ProductoId").HasColumnName("ProductoID");
                        j.IndexerProperty<int>("IngredienteId").HasColumnName("IngredienteID");
                    });
        });

        modelBuilder.Entity<ProductoImagen>(entity =>
        {
            entity.HasKey(e => e.ImagenId).HasName("PK__Producto__0C7D20D7AC6FE84F");

            entity.Property(e => e.ImagenId).HasColumnName("ImagenID");
            entity.Property(e => e.FechaSubida)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.RutaImagen)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Producto).WithMany(p => p.ProductoImagen)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductoI__Produ__3F466844");
        });

        modelBuilder.Entity<ProductoPersonalizacion>(entity =>
        {
            entity.HasKey(e => new { e.ProductoId, e.OpcionId }).HasName("PK__Producto__834C7E03CD17ECF4");

            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.OpcionId).HasColumnName("OpcionID");

            entity.HasOne(d => d.Opcion).WithMany(p => p.ProductoPersonalizacion)
                .HasForeignKey(d => d.OpcionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductoP__Opcio__4BAC3F29");

            entity.HasOne(d => d.Producto).WithMany(p => p.ProductoPersonalizacion)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductoP__Produ__4AB81AF0");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Rol__F92302D19B38C7AC");

            entity.HasIndex(e => e.Nombre, "UQ__Rol__75E3EFCF21CB04C0").IsUnique();

            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuario__2B3DE7984C7223F4");

            entity.HasIndex(e => e.Correo, "UQ__Usuario__60695A19E765E005").IsUnique();

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TokenExpiracion).HasColumnType("datetime");
            entity.Property(e => e.TokenRecuperacion)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuario__RolID__286302EC");
        });

        modelBuilder.Entity<ValorPersonalizacion>(entity =>
        {
            entity.HasKey(e => e.ValorId).HasName("PK__ValorPer__5DDD71E4DE1A5E98");

            entity.Property(e => e.ValorId).HasColumnName("ValorID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OpcionId).HasColumnName("OpcionID");

            entity.HasOne(d => d.Opcion).WithMany(p => p.ValorPersonalizacion)
                .HasForeignKey(d => d.OpcionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ValorPers__Opcio__46E78A0C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
