using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.DTOs;
using BrewTime.Application.Security;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Models;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceAutenticacion : IServiceAutenticacion
    {
        private readonly IRepositoryUsuario _repository;

        public ServiceAutenticacion(IRepositoryUsuario repository)
        {
            _repository = repository;
        }

        public async Task<Usuario?> LoginAsync(LoginDTO dto)
        {
            // buscar usuario por correo
            var usuario = await _repository.FindByCorreoAsync(dto.Correo);

            if (usuario == null)
                return null;

            // validar contraseña
            bool passwordCorrecta = PasswordHelper.VerifyPassword(usuario, dto.Password);

            if (!passwordCorrecta)
                return null;

            return usuario;
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            return await _repository.FindByIdAsync(id);
        }
    }
}

