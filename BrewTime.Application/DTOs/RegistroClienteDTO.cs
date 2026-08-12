using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
      public record RegistroClienteDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 2,
         ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Los apellidos deben tener entre 2 y 100 caracteres.")]
        public string Apellidos { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        public string Correo { get; set; } = null!;

        [Phone(ErrorMessage = "Ingrese un número de teléfono válido.")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener mínimo 8 caracteres.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "La contraseña debe contener mayúscula, minúscula y número.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [DataType(DataType.Password)]
        public string ConfirmarPassword { get; set; } = null!;
    }
}
