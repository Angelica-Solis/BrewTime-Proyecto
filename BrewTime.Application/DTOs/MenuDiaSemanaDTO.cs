using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BrewTime.Application.DTOs
{
    public record MenuDiaSemanaDTO
    {
        public int MenuId { get; set; }

        [Display(Name = "Día de Semana")]
        public byte DiaSemana { get; set; }

        public virtual MenuDTO Menu { get; set; } = null!;
    }
}
