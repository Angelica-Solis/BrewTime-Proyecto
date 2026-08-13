using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BrewTime.Application.DTOs
{
    public class ComboFormDTO
    {
        public int ComboID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio especial es obligatorio.")]
        [Range(1, 999999, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal PrecioEspecial { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public int CategoriaID { get; set; }

        public bool Activo { get; set; } = true;

        public string? RutaImagenActual { get; set; }
        public IFormFile? Imagen { get; set; }
        public bool EliminarImagen { get; set; }

        // IDs de productos seleccionados con sus cantidades
        public List<ComboProductoFormDTO> ProductosSeleccionados { get; set; } = new();
    }
}
