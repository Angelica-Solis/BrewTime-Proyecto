using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;
using System.ComponentModel.DataAnnotations;

namespace BrewTime.Application.DTOs
{
    public record MenuDTO
    {
        public int MenuId { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Hora Inicio")]
        public TimeSpan HoraInicio { get; set; }

        [Display(Name = "Hora Fin")]
        public TimeSpan HoraFin { get; set; }

        [Display(Name = "Fecha Inicio")]
        public DateOnly? FechaInicio { get; set; }

        [Display(Name = "Fecha Fin")]
        public DateOnly? FechaFin { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; }

        public bool Activo { get; set; }

        public virtual List<MenuDiaSemanaDTO> MenuDiaSemana { get; set; } = new();
        public virtual List<MenuProductoDTO> MenuProducto { get; set; } = new();
        public virtual List<MenuComboDTO> MenuCombo { get; set; } = new();
    }
}
