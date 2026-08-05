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
    public class RepositoryCarrito : IRepositoryCarrito
    {
        private readonly BrewTimeContext _context;

        public RepositoryCarrito(BrewTimeContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Carrito>> GetByUsuarioAsync(int usuarioId)
        {
            return await _context.Carrito
                .Include(c => c.Producto)
                    .ThenInclude(p => p.ProductoImagen.Where(i => i.EsPrincipal))
                .Include(c => c.Combo)
                .Where(c => c.UsuarioId == usuarioId)
                .OrderBy(c => c.FechaAgregado)
                .ToListAsync();
        }

        public async Task<Carrito?> GetProductoAsync(int usuarioId, int productoId)
        {
            return await _context.Carrito
                .FirstOrDefaultAsync(c =>
                    c.UsuarioId == usuarioId &&
                    c.ProductoId == productoId);
        }

        public async Task<Carrito?> GetComboAsync(int usuarioId, int comboId)
        {
            return await _context.Carrito
                .FirstOrDefaultAsync(c =>
                    c.UsuarioId == usuarioId &&
                    c.ComboId == comboId);
        }

        public async Task<Carrito?> FindByIdAsync(int carritoId)
        {
            return await _context.Carrito
                .Include(c => c.Producto)
                .Include(c => c.Combo)
                .FirstOrDefaultAsync(c => c.CarritoId == carritoId);
        }

        public async Task AddAsync(Carrito carrito)
        {
            await _context.Carrito.AddAsync(carrito);
        }

        public Task UpdateAsync(Carrito carrito)
        {
            _context.Carrito.Update(carrito);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int carritoId)
        {
            var carrito = await FindByIdAsync(carritoId);

            if (carrito != null)
            {
                _context.Carrito.Remove(carrito);
            }
        }

        public async Task DeleteAllAsync(int usuarioId)
        {
            var lista = await _context.Carrito
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            _context.Carrito.RemoveRange(lista);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> CantidadItemsAsync(int usuarioId)
        {
            return await _context.Carrito
                .Where(c => c.UsuarioId == usuarioId)
                .SumAsync(c => c.Cantidad);
        }
    }
}

