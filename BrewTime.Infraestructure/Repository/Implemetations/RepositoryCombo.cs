using BrewTime.Infraestructure.Data;
using BrewTime.Infraestructure.Models;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewTime.Infraestructure.Repository.Implemetations
{
    public class RepositoryCombo : IRepositoryCombo
    {
        private readonly BrewTimeContext _context;

        public RepositoryCombo(BrewTimeContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Combo>> ListAsync()
        {
            var collection = await _context.Set<Combo>()
                .Include(c => c.Categoria)
                .Where(c => c.Activo == true)
                .ToListAsync();

            return collection;
        }

        public async Task<Combo> FindByIdAsync(int id)
        {
            var entity = await _context.Set<Combo>()
                .Include(c => c.Categoria)
                .Include(c => c.ComboProducto)
                    .ThenInclude(cp => cp.Producto)
                .FirstOrDefaultAsync(c => c.ComboId == id);

            return entity!;
        }
        public Task CreateAsync(Combo entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Combo entity)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Combo>> ListInactivosAsync()
        {
            return await _context.Set<Combo>()
                .Include(c => c.Categoria)
                .Where(c => c.Activo == false)
                .ToListAsync();
        }

        public async Task ToggleActivoAsync(int id)
        {
            var entity = await _context.Set<Combo>()
                .FirstOrDefaultAsync(c => c.ComboId == id);

            if (entity != null)
            {
                entity.Activo = !entity.Activo;
                _context.Set<Combo>().Update(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}