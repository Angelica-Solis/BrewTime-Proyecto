using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryUsuario
    {
        Task<ICollection<Usuario>> ListAsync();
        Task<Usuario> FindByIdAsync(int id);
        // encontrar usuario por correo
        Task<Usuario?> FindByCorreoAsync(string correo);
        Task<Rol?> FindRolByNombreAsync(string nombreRol);

        Task<bool> ExistsByCorreoAsync(string correo);

        Task AddAsync(Usuario usuario);

        Task UpdateAsync(Usuario usuario);
    }
}
