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
    public class RepositoryMenu : IRepositoryMenu
    {
        private readonly BrewTimeContext _context;

        public RepositoryMenu(BrewTimeContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Menu>> ListAsync()
        {
            return await _context.Set<Menu>()
                .Include(x => x.MenuDiaSemana)

                .Include(x => x.MenuProducto)
                    .ThenInclude(x => x.Producto)
                        .ThenInclude(x => x.Categoria)

                .Include(x => x.MenuCombo)
                    .ThenInclude(x => x.Combo)
                        .ThenInclude(x => x.Categoria)

                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Menu> FindByIdAsync(int id)
        {
            return await _context.Set<Menu>()
                .Include(x => x.MenuDiaSemana)

                .Include(x => x.MenuProducto)
                    .ThenInclude(x => x.Producto)
                        .ThenInclude(x => x.Categoria)

                .Include(x => x.MenuCombo)
                    .ThenInclude(x => x.Combo)
                        .ThenInclude(x => x.Categoria)

                .FirstAsync(x => x.MenuId == id);
        }
    }
}
