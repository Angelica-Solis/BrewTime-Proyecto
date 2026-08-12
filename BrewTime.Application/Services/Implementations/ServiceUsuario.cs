using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Application.Security;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Models;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceUsuario : IServiceUsuario
    {
        private readonly IRepositoryUsuario _repository;
        private readonly IMapper _mapper;
        public ServiceUsuario(IRepositoryUsuario repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<UsuarioDetalleDTO> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<UsuarioDetalleDTO>(@object);
            return objectMapped;
        }

        public async Task<ICollection<UsuarioDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();

            var collection = _mapper.Map<ICollection<UsuarioDTO>>(list);

            return collection;
        }
        public async Task<(bool Exito, string Mensaje)>RegistrarClienteAsync(RegistroClienteDTO dto)
        {
            dto.Correo = dto.Correo.Trim();

            if (await _repository.ExistsByCorreoAsync(dto.Correo))
            {
                return (false, "El correo electrónico ya se encuentra registrado."
                );
            }

            var rolCliente = await _repository.FindRolByNombreAsync("Cliente");

            if (rolCliente == null)
            {
                return (false, "No existe el rol Cliente en la base de datos."
                );
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Apellidos = dto.Apellidos.Trim(),
                Correo = dto.Correo,
                Telefono = string.IsNullOrWhiteSpace(dto.Telefono)
                    ? null
                    : dto.Telefono.Trim(),
                RolId = rolCliente.RolId,
                FechaRegistro = DateTime.Now,
                Activo = true,
                ContrasenaTemp = false
            };

            usuario.PasswordHash = PasswordHelper.HashPassword(usuario, dto.Password);

            await _repository.AddAsync(usuario);

            return (true, "El cliente fue registrado correctamente.");
        }

        public async Task<(bool Exito, string Mensaje)>CrearEmpleadoAsync(UsuarioCreateDTO dto)
        {
            dto.Correo = dto.Correo.Trim();

            if (await _repository.ExistsByCorreoAsync(dto.Correo))
            {
                return (false,"El correo electrónico ya se encuentra registrado.");
            }

            var rolesPermitidos = new[]
            {
                "Encargado",
                "Cocina"
            };

            if (!rolesPermitidos.Contains(dto.NombreRol))
            {
                return (false, "Solo se pueden crear usuarios Encargado o Cocina.");
            }

            var rol = await _repository.FindRolByNombreAsync(dto.NombreRol);

            if (rol == null)
            {
                return (false, $"No existe el rol {dto.NombreRol}."
                );
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Apellidos = dto.Apellidos.Trim(),
                Correo = dto.Correo,
                Telefono = string.IsNullOrWhiteSpace(dto.Telefono)
                    ? null
                    : dto.Telefono.Trim(),
                RolId = rol.RolId,
                FechaRegistro = DateTime.Now,
                Activo = true,
                ContrasenaTemp = false
            };

            usuario.PasswordHash = PasswordHelper.HashPassword(usuario, dto.Password);

            await _repository.AddAsync(usuario);

            return (true, $"Usuario {dto.NombreRol} creado correctamente.");
        }

        public async Task<UsuarioEditDTO?> ObtenerParaEditarAsync(int id)
        {
            var usuario = await _repository.FindByIdAsync(id);

            if (usuario == null)
                return null;

            return new UsuarioEditDTO
            {
                UsuarioId = usuario.UsuarioId,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos,
                Correo = usuario.Correo,
                Telefono = usuario.Telefono,
                NombreRol = usuario.Rol.Nombre,
                Activo = usuario.Activo
            };
        }

        public async Task<(bool Exito, string Mensaje)>
            EditarAsync(UsuarioEditDTO dto)
        {
            var usuario = await _repository.FindByIdAsync(dto.UsuarioId);

            if (usuario == null)
            {
                return (false, "El usuario no existe.");
            }

            var correoExiste = await _repository.ExistsByCorreoAsync(dto.Correo.Trim());

            if (correoExiste && !string.Equals(usuario.Correo,dto.Correo.Trim(),StringComparison.OrdinalIgnoreCase))
            {
                return (false, "El correo electrónico ya está registrado.");
            }

            var rolesPermitidos = new[]
            {
                "Cliente",
                "Encargado",
                "Cocina"
            };

            if (!rolesPermitidos.Contains(dto.NombreRol))
            {
                return (false, "El rol seleccionado no está permitido.");
            }

            var rol = await _repository.FindRolByNombreAsync(dto.NombreRol);

            if (rol == null)
            {
                return (false, "El rol seleccionado no existe.");
            }

            usuario.Nombre = dto.Nombre.Trim();
            usuario.Apellidos = dto.Apellidos.Trim();
            usuario.Correo = dto.Correo.Trim();
            usuario.Telefono =
                string.IsNullOrWhiteSpace(dto.Telefono)
                    ? null
                    : dto.Telefono.Trim();

            usuario.RolId = rol.RolId;
            usuario.Activo = dto.Activo;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                usuario.PasswordHash =
                    PasswordHelper.HashPassword(
                        usuario,
                        dto.Password);

                usuario.ContrasenaTemp = false;
            }

            await _repository.UpdateAsync(usuario);

            return (true, "El usuario fue actualizado correctamente."
            );
        }

        public async Task<ICollection<string>>ObtenerRolesAdministrativosAsync()
        {
            var roles = new List<string>
            {
                "Encargado",
                "Cocina"
            };

            return await Task.FromResult(roles);
        }
    }
}
