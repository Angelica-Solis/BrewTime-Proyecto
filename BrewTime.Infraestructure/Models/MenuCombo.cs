using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BrewTime.Infraestructure.Models
{
    public partial class MenuCombo
    {
        public int MenuId { get; set; }

        public int ComboId { get; set; }

        public virtual Combo Combo { get; set; } = null!;

        public virtual Menu Menu { get; set; } = null!;
    }
}
