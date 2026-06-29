using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BrewTime.Application.DTOs
{
    public class ProductoFormDTO
    {
        public int ProductoID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(1, 999999, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public int CategoriaID { get; set; }

        public bool Activo { get; set; } = true;

        // Imágenes actuales (para mostrar en Edit)
        public List<ProductoImagenDTO> ImagenesActuales { get; set; } = new();

        // IDs marcados para eliminar de BD
        public List<int> ImagenesAEliminar { get; set; } = new();

        // Archivos nuevos subidos por el usuario
        public List<IFormFile>? ImagenesNuevas { get; set; }
    }

    public class ProductoImagenDTO
    {
        public int ImagenID { get; set; }
        public string RutaImagen { get; set; } = null!;
        public bool EsPrincipal { get; set; }
    }
}