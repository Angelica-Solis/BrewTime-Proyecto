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
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Hora de inicio")]
        public TimeOnly HoraInicio { get; set; }

        [Display(Name = "Hora final")]
        public TimeOnly HoraFin { get; set; }

        [Display(Name = "Fecha de inicio")]
        public DateOnly? FechaInicio { get; set; }

        [Display(Name = "Fecha final")]
        public DateOnly? FechaFin { get; set; }

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        public List<MenuDiaSemanaDTO> MenuDiaSemana { get; set; } = new();
        public List<MenuProductoDTO> MenuProducto { get; set; } = new();
        public List<MenuComboDTO> MenuCombo { get; set; } = new();
    }
}
