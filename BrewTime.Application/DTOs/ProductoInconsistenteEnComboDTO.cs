using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record ProductoInconsistenteEnComboDTO
    {
        public int ComboID { get; set; }
        public string NombreCombo { get; set; } = null!;
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; } = null!;
        public string MotivoInconsistencia { get; set; } = null!; // "Producto inactivo" o "Producto sin ingredientes"
    }
}
