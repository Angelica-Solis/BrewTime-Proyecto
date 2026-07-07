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
    public class RepositoryIngrediente : IRepositoryIngrediente
    {
        private readonly BrewTimeContext _context;

        public RepositoryIngrediente(BrewTimeContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Ingrediente>> ListActivasAsync()
        {
            return await _context.Set<Ingrediente>()
                .Where(i => i.Activo == true)
                .OrderBy(i => i.Nombre)
                .ToListAsync();
        }

        public async Task<ICollection<Ingrediente>> FindByIdsAsync(IEnumerable<int> ids)
        {
            var listaIds = ids?.ToList() ?? new List<int>();

            if (!listaIds.Any())
                return new List<Ingrediente>();

            return await _context.Set<Ingrediente>()
                .Where(i => listaIds.Contains(i.IngredienteId))
                .ToListAsync();
        }

        public async Task<Ingrediente> ObtenerOCrearAsync(string nombre)
        {
            var nombreLimpio = nombre.Trim();

            var existente = await _context.Set<Ingrediente>()
                .FirstOrDefaultAsync(i => i.Nombre.ToLower() == nombreLimpio.ToLower());

            if (existente != null)
                return existente;

            var nuevoIngrediente = new Ingrediente
            {
                Nombre = nombreLimpio,
                Activo = true
            };

            // Cada ingrediente nuevo se inserta en su propia operación (un INSERT por ingrediente),
            // tal como lo requiere la tabla Ingrediente de la base de datos.
            await _context.Set<Ingrediente>().AddAsync(nuevoIngrediente);
            await _context.SaveChangesAsync();

            return nuevoIngrediente;
        }
    }
}
