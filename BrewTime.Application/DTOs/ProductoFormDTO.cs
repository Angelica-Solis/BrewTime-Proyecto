using System.ComponentModel.DataAnnotations;

namespace BrewTime.Application.DTOs
{
    public record ProductoFormDTO
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
    }
}