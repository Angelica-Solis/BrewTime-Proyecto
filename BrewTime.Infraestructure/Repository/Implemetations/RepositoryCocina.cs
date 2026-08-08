using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Data;
using BrewTime.Infraestructure.Models;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewTime.Infraestructure.Repository.Implemetations
{
    public class RepositoryCocina : IRepositoryCocina
    {
        private readonly BrewTimeContext _context;

        public RepositoryCocina(BrewTimeContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pedido>> ObtenerPedidosAceptadosAsync()
        {
            return await _context.Pedido
                .AsNoTracking()
                .Where(p => p.Estado.Nombre == "Aceptada")
                .OrderBy(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<bool> CambiarAEnPreparacionAsync(int pedidoId)
        {
            var pedido = await _context.Pedido
                .Include(p => p.Estado)
                .FirstOrDefaultAsync(p =>
                    p.PedidoId == pedidoId &&
                    p.Estado.Nombre == "Aceptada");

            if (pedido == null)
                return false;

            var estadoPreparacion = await _context.EstadoPedido
                .FirstOrDefaultAsync(e => e.Nombre == "En preparación");

            if (estadoPreparacion == null)
                return false;

            pedido.EstadoId = estadoPreparacion.EstadoId;
            pedido.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Pedido>> ObtenerPedidosEnPreparacionAsync()
        {
            return await _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.Estado)

                .Include(p => p.PedidoDetalle)
                    .ThenInclude(d => d.Producto)
                        .ThenInclude(p => p.Categoria)

                .Include(p => p.PedidoDetalle)
                    .ThenInclude(d => d.Producto)
                        .ThenInclude(p => p.ProcesoPreparacion)
                            .ThenInclude(pp => pp.Estacion)

                .Include(p => p.PedidoDetalle)
                    .ThenInclude(d => d.Combo)
                        .ThenInclude(c => c.ComboProducto)
                            .ThenInclude(cp => cp.Producto)
                                .ThenInclude(p => p.Categoria)

                .Include(p => p.PedidoDetalle)
                    .ThenInclude(d => d.Combo)
                        .ThenInclude(c => c.ComboProducto)
                            .ThenInclude(cp => cp.Producto)
                                .ThenInclude(p => p.ProcesoPreparacion)
                                    .ThenInclude(pp => pp.Estacion)

                .Include(p => p.PedidoDetalle)
                    .ThenInclude(d => d.ColaEstacion)
                        .ThenInclude(c => c.Estacion)

                .Where(p => p.Estado.Nombre == "En preparación")
                .OrderBy(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<bool> MarcarPedidoEnCaminoAsync(int pedidoId)
        {
            var pedido = await _context.Pedido
                .Include(p => p.Estado)
                .FirstOrDefaultAsync(p =>
                    p.PedidoId == pedidoId &&
                    p.Estado.Nombre == "En preparación");

            if (pedido == null)
                return false;

            var estadoEnCamino = await _context.EstadoPedido
                .FirstOrDefaultAsync(e => e.Nombre == "En camino");

            if (estadoEnCamino == null)
                return false;

            pedido.EstadoId = estadoEnCamino.EstadoId;
            pedido.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}

