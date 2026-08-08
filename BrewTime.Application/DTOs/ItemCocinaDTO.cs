using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public class ItemCocinaDTO
    {
        public int DetalleId { get; set; }

        public int ProductoId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public bool EsCombo { get; set; }

        public string? NombreCombo { get; set; }

        public string Categoria { get; set; } = string.Empty;

        public List<EstacionCocinaDTO> Estaciones { get; set; } = new();
    }
}

