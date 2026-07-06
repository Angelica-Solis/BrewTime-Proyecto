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
    public class RepositoryUsuario : IRepositoryUsuario
    {
        private readonly BrewTimeContext _context;
        public RepositoryUsuario(BrewTimeContext context)
        {
            _context = context;
        }
        public async Task<Usuario> FindByIdAsync(int id)
        {
            var entity = await _context.Set<Usuario>()
                .Include(p => p.Rol)
                .FirstOrDefaultAsync(p => p.UsuarioId == id);

            return entity!;
        }

        public async Task<ICollection<Usuario>> ListAsync()
        {
            var collection = await _context.Set<Usuario>()
               .Include(p => p.Rol)
               .ToListAsync();

            return collection;
        }
    }
}
