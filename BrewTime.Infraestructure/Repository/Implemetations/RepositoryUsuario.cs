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
        public async Task<Usuario?> FindByCorreoAsync(string correo)
        {
            return await _context.Usuario
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == correo && u.Activo);
        }

        public async Task<bool> ExistsByCorreoAsync(string correo)
        {
            correo = correo.Trim();

            return await _context.Usuario
                .AnyAsync(u => u.Correo == correo);
        }

        public async Task<Rol?> FindRolByNombreAsync(string nombreRol)
        {
            nombreRol = nombreRol.Trim();

            return await _context.Rol
                .FirstOrDefaultAsync(r => r.Nombre == nombreRol);
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
