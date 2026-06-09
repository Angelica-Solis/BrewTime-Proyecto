using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BrewTime.Application.DTOs
{
    public record MenuComboDTO
    {
        public int MenuId { get; set; }

        [Display(Name = "Combo")]
        public int ComboId { get; set; }

        public virtual MenuDTO Menu { get; set; } = null!;
        public virtual ComboDTO Combo { get; set; } = null!;
    }
}
