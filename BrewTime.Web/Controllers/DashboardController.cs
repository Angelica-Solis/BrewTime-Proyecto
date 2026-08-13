using BrewTime.Application.DTOs;
using BrewTime.Infraestructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrewTime.Web.Controllers;

[Authorize(Roles = "Administrador,Encargado")]
public class DashboardController : Controller
{
    private readonly BrewTimeContext _context;
    public DashboardController(BrewTimeContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var inicio = DateTime.Today;
        var fin = inicio.AddDays(1);
        var productos = await _context.PedidoDetalle
            .Where(d => d.ProductoId != null && d.Pedido.FechaCreacion >= inicio && d.Pedido.FechaCreacion < fin)
            .GroupBy(d => d.Producto!.Nombre)
            .Select(g => new DatoReporteDTO { Etiqueta = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
            .OrderByDescending(x => x.Cantidad).ThenBy(x => x.Etiqueta).Take(3).ToListAsync();
        var estados = await _context.EstadoPedido
            .OrderBy(e => e.EstadoId)
            .Select(e => new DatoReporteDTO {
                Etiqueta = e.Nombre,
                Cantidad = e.Pedido.Count(p => p.FechaCreacion >= inicio && p.FechaCreacion < fin)
            }).ToListAsync();
        return View(new ReporteDashboardDTO { Fecha = inicio, ProductosMasPedidos = productos, PedidosPorEstado = estados });
    }
}