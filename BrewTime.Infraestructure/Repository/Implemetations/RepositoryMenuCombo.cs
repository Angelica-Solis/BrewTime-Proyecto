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
    public class RepositoryMenuCombo : IRepositoryMenuCombo
    {
        private readonly BrewTimeContext _context;

        public RepositoryMenuCombo(BrewTimeContext context)
        {
            _context = context;
        }

        public async Task<ICollection<MenuCombo>> ListAsync()
        {
            return await _context.Set<MenuCombo>()
                .Include(x => x.Menu)
                .Include(x => x.Combo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MenuCombo> FindByIdAsync(int menuId, int comboId)
        {
            return await _context.Set<MenuCombo>()
                .Include(x => x.Menu)
                .Include(x => x.Combo)
                .FirstAsync(x => x.MenuId == menuId && x.ComboId == comboId);
        }
    }
}
