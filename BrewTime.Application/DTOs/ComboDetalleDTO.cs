namespace BrewTime.Application.DTOs
{
    public record ComboDetalleDTO
    {
        public int ComboID { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal PrecioEspecial { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string CategoriaNombre { get; set; } = null!;
        public List<ComboProductoDTO> Productos { get; set; } = new();
    }
}