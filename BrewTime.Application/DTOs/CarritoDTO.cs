using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public record CarritoDTO
    {
        public List<CarritoItemDTO> Items { get; set; } = new();

        public decimal Subtotal
        {
            get
            {
                return Items.Sum(x => x.Subtotal);
            }
        }

        public decimal IVA
        {
            get
            {
                return Subtotal * 0.13m;
            }
        }

        public decimal Total
        {
            get
            {
                return Subtotal + IVA;
            }
        }
    }
}

