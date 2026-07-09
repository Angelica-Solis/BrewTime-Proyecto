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
    public class RepositoryProducto : IRepositoryProducto
    {
        private readonly BrewTimeContext _context;
        public RepositoryProducto(BrewTimeContext context)
        {
            _context = context;
        }
        public async Task<Producto> FindByIdAsync(int id)
        {
            var entity = await _context.Set<Producto>()
                .Include(p => p.Categoria)
                .Include(p => p.Ingrediente)        
                .Include(p => p.ProductoImagen)     
                .FirstOrDefaultAsync(p => p.ProductoId == id);

            return entity!;
        }
        public async Task<ICollection<Producto>> ListAsync()
        {
            // SELECT con Include para traer la Categoria relacionada
            var collection = await _context.Set<Producto>()
                .Include(p => p.Categoria)
                .Where(p => p.Activo == true)
                .ToListAsync();

            return collection;
        }
        public async Task CreateAsync(Producto entity)
        {
            await _context.Set<Producto>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Producto entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.Set<Producto>().Update(entity);
            }

            await _context.SaveChangesAsync();
        }

        public void DeleteImagen(ProductoImagen imagen)
        {
            _context.Set<ProductoImagen>().Remove(imagen);
        }

        public async Task<ICollection<Producto>> ListInactivosAsync()
        {
            return await _context.Set<Producto>()
                .Include(p => p.Categoria)
                .Where(p => p.Activo == false)
                .ToListAsync();
        }

        public async Task ToggleActivoAsync(int id)
        {
            var entity = await _context.Set<Producto>()
                .FirstOrDefaultAsync(p => p.ProductoId == id);

            if (entity != null)
            {
                // Invierte el estado actual
                entity.Activo = !entity.Activo;
                _context.Set<Producto>().Update(entity);
                await _context.SaveChangesAsync();
            }
        }
        // productos sin procesos
        public async Task<ICollection<Producto>> ProductosSinProcesoAsync()
        {
            return await _context.Producto
                .Where(p =>
                    !_context.ProcesoPreparacion
                    .Any(pp => pp.ProductoId == p.ProductoId)
                )
                .ToListAsync();
        }
    }
}
