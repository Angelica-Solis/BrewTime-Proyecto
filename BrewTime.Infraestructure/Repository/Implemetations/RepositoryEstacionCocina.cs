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
    public class RepositoryEstacionCocina : IRepositoryEstacionCocina
    {
        private readonly BrewTimeContext _context;
        public RepositoryEstacionCocina(BrewTimeContext context)
        {
            _context = context;
        }
        public async Task<ICollection<EstacionCocina>> ListAsync()
        {
            var collection = await _context.Set<EstacionCocina>()
               .Where(p => p.Activo == true)
               .ToListAsync();

            return collection;
        }
    }
}
