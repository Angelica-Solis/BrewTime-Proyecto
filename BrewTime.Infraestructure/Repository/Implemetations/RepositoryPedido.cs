using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Data;
using BrewTime.Infraestructure.Models;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewTime.Infraestructure.Repository.Implemetations
{
    public class RepositoryPedido : IRepositoryPedido
    {
        private readonly BrewTimeContext _context;

        public RepositoryPedido(BrewTimeContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Pedido>> ListAllAsync()
        {
            return await _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.Estado)
                .Include(p => p.MetodoEntrega)
                .Include(p => p.MetodoPago)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<ICollection<Pedido>> ListByClienteAsync(int clienteId)
        {
            return await _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.Estado)
                .Include(p => p.MetodoEntrega)
                .Include(p => p.MetodoPago)
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Pedido?> FindByIdAsync(int id)
        {
            return await _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.Empleado)
                .Include(p => p.Estado)
                .Include(p => p.MetodoEntrega)
                .Include(p => p.MetodoPago)
                .Include(p => p.PedidoDetalle)
                    .ThenInclude(d => d.Producto)
                .Include(p => p.PedidoDetalle)
                    .ThenInclude(d => d.Combo)
                .FirstOrDefaultAsync(p => p.PedidoId == id);
        }

        public async Task<ICollection<EstadoPedido>> ListEstadosAsync()
        {
            return await _context.EstadoPedido.ToListAsync();
        }
    }
}
