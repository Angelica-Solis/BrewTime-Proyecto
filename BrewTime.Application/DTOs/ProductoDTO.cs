using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Application.DTOs
{
    public record ProductoDTO
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Datos de la Categoría (viene del Include)
        public int CategoriaID { get; set; }
        public string CategoriaNombre { get; set; } = null!;
    }
}
