using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record LoginDTO
    {
        [Required(ErrorMessage = "Ingrese el correo")]
        [EmailAddress]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Ingrese la contraseña")]
        public string Password { get; set; }
    }
}
