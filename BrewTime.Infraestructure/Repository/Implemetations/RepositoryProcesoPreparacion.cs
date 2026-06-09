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
    public class RepositoryProcesoPreparacion : IRepositoryProcesoPreparacion
    {
        private readonly BrewTimeContext _context;

        public RepositoryProcesoPreparacion(BrewTimeContext context)
        {
            _context = context;
        }

        public async Task<ICollection<ProcesoPreparacion>> ListAsync()
        {
            var collection = await _context.Set<ProcesoPreparacion>()
                .Include(c => c.Producto)
                .Include(c => c.Estacion)
                .ToListAsync();

            return collection;
        }

        public async Task<ProcesoPreparacion> FindByIdAsync(int id)
        {
            var entity = await _context.Set<ProcesoPreparacion>()
                .Include(c => c.Producto)
                .Include(c => c.Estacion)
                .FirstOrDefaultAsync(c => c.ProcesoId == id);

            return entity!;
        }
    }
}
