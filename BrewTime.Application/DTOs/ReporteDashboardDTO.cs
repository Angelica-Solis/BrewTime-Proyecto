namespace BrewTime.Application.DTOs;

public class ReporteDashboardDTO
{
    public DateTime Fecha { get; set; }
    public List<DatoReporteDTO> ProductosMasPedidos { get; set; } = new();
    public List<DatoReporteDTO> PedidosPorEstado { get; set; } = new();
}

public class DatoReporteDTO
{
    public string Etiqueta { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}