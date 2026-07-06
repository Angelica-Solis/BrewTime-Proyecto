using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record UsuarioDTO
    {
        public int UsuarioId { get; set; }

        public string NombreRol { get; set; }

        public string Nombre { get; set; } = null!;

        public string Apellidos { get; set; } = null!;

        public bool Activo { get; set; }
    }
}
