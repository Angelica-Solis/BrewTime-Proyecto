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
    public class RepositoryMenuProducto : IRepositoryMenuProducto
    {
        private readonly BrewTimeContext _context;

        public RepositoryMenuProducto(BrewTimeContext context)
        {
            _context = context;
        }

        public async Task<ICollection<MenuProducto>> ListAsync()
        {
            return await _context.Set<MenuProducto>()
                .Include(x => x.Menu)
                .Include(x => x.Producto)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MenuProducto> FindByIdAsync(int menuId, int productoId)
        {
            return await _context.Set<MenuProducto>()
                .Include(x => x.Menu)
                .Include(x => x.Producto)
                .FirstAsync(x => x.MenuId == menuId && x.ProductoId == productoId);
        }
    }
}
