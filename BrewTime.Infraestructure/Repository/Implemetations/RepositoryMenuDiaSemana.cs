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
    public class RepositoryMenuDiaSemana : IRepositoryMenuDiaSemana
    {
        private readonly BrewTimeContext _context;

        public RepositoryMenuDiaSemana(BrewTimeContext context)
        {
            _context = context;
        }

        public async Task<ICollection<MenuDiaSemana>> ListAsync()
        {
            return await _context.Set<MenuDiaSemana>()
                .Include(x => x.Menu)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MenuDiaSemana> FindByIdAsync(int menuId, byte diaSemana)
        {
            return await _context.Set<MenuDiaSemana>()
                .Include(x => x.Menu)
                .FirstAsync(x => x.MenuId == menuId && x.DiaSemana == diaSemana);
        }
    }
}
