using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Infraestructure.Models
{
    public partial class Carrito
    {
        public int CarritoId { get; set; }

        public int UsuarioId { get; set; }

        public int? ProductoId { get; set; }

        public int? ComboId { get; set; }

        public int Cantidad { get; set; }

        public DateTime FechaAgregado { get; set; }

        // Navegaciones
        public virtual Usuario Usuario { get; set; } = null!;

        public virtual Producto? Producto { get; set; }

        public virtual Combo? Combo { get; set; }
    }
}
