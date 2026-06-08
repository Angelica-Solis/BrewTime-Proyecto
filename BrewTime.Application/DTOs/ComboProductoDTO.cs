namespace BrewTime.Application.DTOs
{
    public record ComboProductoDTO
    {
        public string NombreProducto { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}