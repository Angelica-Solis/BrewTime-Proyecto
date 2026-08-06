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

        //estado de pedido
        public async Task<ICollection<EstadoPedido>>ListEstadosAsync()
        {
            return await _context.Set<EstadoPedido>().OrderBy(e => e.EstadoId).ToListAsync();
        }

        public async Task<EstadoPedido?> FindEstadoByNombreAsync(string nombre)
        {
            string nombreNormalizado = nombre.Trim().ToLower();

            return await _context.Set<EstadoPedido>().FirstOrDefaultAsync(e => e.Nombre.ToLower() == nombreNormalizado);
        }

        //metodos de entrega
        public async Task<ICollection<MetodoEntrega>>ListMetodosEntregaAsync()
        {
            return await _context.Set<MetodoEntrega>().OrderBy(m => m.MetodoId).ToListAsync();
        }

        public async Task<MetodoEntrega?>FindMetodoEntregaByIdAsync(int metodoEntregaId)
        {
            return await _context.Set<MetodoEntrega>().FirstOrDefaultAsync(m => m.MetodoId == metodoEntregaId);
        }

        //metodos de pago
        public async Task<ICollection<MetodoPago>>ListMetodosPagoAsync()
        {
            return await _context.Set<MetodoPago>().OrderBy(m => m.MetodoPagoId).ToListAsync();
        }

        public async Task<MetodoPago?>FindMetodoPagoByIdAsync(int metodoPagoId)
        {
            return await _context.Set<MetodoPago>().FirstOrDefaultAsync(m => m.MetodoPagoId == metodoPagoId);
        }

        //registrar pedido
        public async Task CreateAsync(Pedido pedido)
        {
            await _context.Set<Pedido>().AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        //actualizar pedido y pago
        public async Task UpdateAsync(Pedido pedido)
        {

            if (_context.Entry(pedido).State == EntityState.Detached)
            {
                _context.Set<Pedido>().Update(pedido);
            }

            await _context.SaveChangesAsync();
        }
    }
}
