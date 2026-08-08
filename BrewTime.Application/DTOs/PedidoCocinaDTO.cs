using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public class PedidoCocinaDTO
    {
        public int PedidoId { get; set; }

        public string NombreCliente { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }

        public string Estado { get; set; } = string.Empty;

        public List<ItemCocinaDTO> Comida { get; set; } = new();

        public List<ItemCocinaDTO> Cafe { get; set; } = new();

        public List<ItemCocinaDTO> BubbleTea { get; set; } = new();
    }
}
