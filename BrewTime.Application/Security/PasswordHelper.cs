using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;
using Microsoft.AspNetCore.Identity;

namespace BrewTime.Application.Security
{
    public class PasswordHelper
    {
        private static readonly PasswordHasher<Usuario> hasher = new();

        public static string HashPassword(Usuario usuario, string password)
        {
            return hasher.HashPassword(usuario, password);
        }

        public static bool VerifyPassword(Usuario usuario, string password)
        {
            var result = hasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                password);

            return result == PasswordVerificationResult.Success;
        }
    }
}

