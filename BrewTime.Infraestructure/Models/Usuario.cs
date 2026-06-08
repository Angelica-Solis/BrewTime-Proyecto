using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public int RolId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Telefono { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool Activo { get; set; }

    public string? TokenRecuperacion { get; set; }

    public DateTime? TokenExpiracion { get; set; }

    public bool ContrasenaTemp { get; set; }

    public virtual ICollection<ColaEstacion> ColaEstacion { get; set; } = new List<ColaEstacion>();

    public virtual ICollection<HistorialConsumo> HistorialConsumo { get; set; } = new List<HistorialConsumo>();

    public virtual ICollection<Pedido> PedidoCliente { get; set; } = new List<Pedido>();

    public virtual ICollection<Pedido> PedidoEmpleado { get; set; } = new List<Pedido>();

    public virtual ICollection<PedidoHistorialEstado> PedidoHistorialEstado { get; set; } = new List<PedidoHistorialEstado>();

    public virtual Rol Rol { get; set; } = null!;
}
