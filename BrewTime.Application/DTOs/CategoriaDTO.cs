namespace BrewTime.Application.DTOs
{
    public class CategoriaDTO
    {
        public int CategoriaID { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
