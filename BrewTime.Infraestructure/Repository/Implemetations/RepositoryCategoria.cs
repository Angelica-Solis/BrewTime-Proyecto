using BrewTime.Infraestructure.Data;
using BrewTime.Infraestructure.Models;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewTime.Infraestructure.Repository.Implemetations
{
    public class RepositoryCategoria : IRepositoryCategoria
    {
        private readonly BrewTimeContext _context;

        public RepositoryCategoria(BrewTimeContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Categoria>> ListActivasAsync()
        {
            return await _context.Set<Categoria>()
                .Where(c => c.Activo == true)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }
    }
}
