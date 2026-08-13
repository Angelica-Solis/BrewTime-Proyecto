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

        public async Task<LoginResult> LoginAsync(LoginDTO dto)
        {
            var usuario = await _repository.FindByCorreoAsync(dto.Correo);

            if (usuario == null)
            {
                return new LoginResult
                {
                    Exitoso = false,
                    Mensaje = "El correo ingresado no está registrado."
                };
            }

            bool passwordCorrecta =
                PasswordHelper.VerifyPassword(usuario, dto.Password);

            if (!passwordCorrecta)
            {
                return new LoginResult
                {
                    Exitoso = false,
                    Mensaje = "La contraseña ingresada es incorrecta."
                };
            }

            return new LoginResult
            {
                Exitoso = true,
                Usuario = usuario
            };
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            return await _repository.FindByIdAsync(id);
        }
    }
}

