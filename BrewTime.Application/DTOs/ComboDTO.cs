namespace BrewTime.Application.DTOs
{
    public record ComboDTO
    {
        public int ComboID { get; set; }
        public string Nombre { get; set; } = null!;
        public string CategoriaNombre { get; set; } = null!;
        public decimal PrecioEspecial { get; set; }
    }
}