using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BrewTime.Application.DTOs
{
    public record MenuProductoDTO
    {
        public int MenuId { get; set; }

        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        public virtual MenuDTO Menu { get; set; } = null!;
        public virtual ProductoDTO Producto { get; set; } = null!;
    }
}
