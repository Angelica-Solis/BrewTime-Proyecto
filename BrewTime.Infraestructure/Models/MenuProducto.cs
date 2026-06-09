using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Infraestructure.Models
{
    public partial class MenuProducto
    {
        public int MenuId { get; set; }

        public int ProductoId { get; set; }

        public virtual Menu Menu { get; set; } = null!;

        public virtual Producto Producto { get; set; } = null!;
    }
}
