using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.DTOs;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceUsuario
    {
        Task<ICollection<UsuarioDTO>> ListAsync();
        Task<UsuarioDetalleDTO> FindByIdAsync(int id);
        Task<(bool Exito, string Mensaje)> RegistrarClienteAsync(RegistroClienteDTO dto);

        Task<(bool Exito, string Mensaje)> CrearEmpleadoAsync(UsuarioCreateDTO dto);

        Task<UsuarioEditDTO?> ObtenerParaEditarAsync(int id);

        Task<(bool Exito, string Mensaje)> EditarAsync(UsuarioEditDTO dto);

        Task<ICollection<string>> ObtenerRolesAdministrativosAsync();
    }
}
