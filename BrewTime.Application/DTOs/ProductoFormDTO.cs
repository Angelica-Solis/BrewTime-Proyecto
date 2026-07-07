using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BrewTime.Application.DTOs
{
    public class ProductoFormDTO : IValidatableObject
    {
        public int ProductoID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
        public string Descripcion { get; set; } = null!;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(1, 999999, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría.")]
        public int CategoriaID { get; set; }

        public bool Activo { get; set; } = true;

        // Imágenes actuales (para mostrar en Edit)
        public List<ProductoImagenDTO> ImagenesActuales { get; set; } = new();

        // Cantidad de imágenes actuales, enviada como campo oculto en el form de Edit.
        // Se necesita porque ImagenesActuales no viaja de vuelta en el POST (no tiene inputs propios),
        // por lo que no se puede confiar en su Count() al validar el formulario enviado.
        public int ImagenesActualesCount { get; set; }

        // IDs marcados para eliminar de BD
        public List<int> ImagenesAEliminar { get; set; } = new();

        // Archivos nuevos subidos por el usuario
        public List<IFormFile>? ImagenesNuevas { get; set; }

        // IDs de ingredientes ya existentes en el catálogo que el usuario marcó
        public List<int> IngredientesSeleccionados { get; set; } = new();

        // Nombres de ingredientes nuevos escritos por el usuario.
        // Cada uno se inserta por separado en la tabla Ingrediente antes de asociarlo al producto.
        public List<string> IngredientesNuevos { get; set; } = new();

        // Validación cruzada: el producto siempre debe quedar con al menos una imagen.
        // - En Create: ImagenesActuales está vacío, así que exige ImagenesNuevas.
        // - En Edit: exige que lo que quede (actuales - eliminadas + nuevas) sea >= 1.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var nuevas = ImagenesNuevas?.Count(f => f != null && f.Length > 0) ?? 0;
            var actuales = ImagenesActualesCount;
            var aEliminar = ImagenesAEliminar?.Count ?? 0;
            var restantes = actuales - aEliminar + nuevas;

            if (restantes <= 0)
            {
                yield return new ValidationResult(
                    "El producto debe tener al menos una imagen.",
                    new[] { nameof(ImagenesNuevas) });
            }
        }
    }
}