using System.ComponentModel.DataAnnotations;

namespace BrewTime.Application.DTOs
{
    public class ComboProductoFormDTO
    {
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; } = null!;
        public int Cantidad { get; set; } = 1;
        public bool Seleccionado { get; set; }
    }
}