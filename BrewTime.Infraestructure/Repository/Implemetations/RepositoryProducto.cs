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
    }
}
