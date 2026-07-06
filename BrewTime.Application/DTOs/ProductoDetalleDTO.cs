using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record ProductoDetalleDTO
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Categoría
        public int CategoriaID { get; set; }
        public string CategoriaNombre { get; set; } = null!;

        // Ingredientes
        public List<string> Ingredientes { get; set; } = new();

        // Imagenes del producto
        public List<string> Imagenes { get; set; } = new();
    }
}
