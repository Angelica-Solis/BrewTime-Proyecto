using System;
using System.Collections.Generic;

namespace BrewTime.Infraestructure.Models;

public partial class MenuDiaSemana
{
    public int MenuId { get; set; }

    public byte DiaSemana { get; set; }

    public virtual Menu Menu { get; set; } = null!;
}
