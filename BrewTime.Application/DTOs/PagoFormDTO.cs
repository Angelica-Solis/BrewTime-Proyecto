using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public class PagoFormDTO
    {
    
        public PedidoDetalleDTO Pedido { get; set; } = new(); 
        public PagoPedidoDTO Pago { get; set; } = new(); //datos que ingresará el usuario para pagar
        public List<MetodoPagoDTO> MetodosPago { get; set; } = new(); //opciones obtenidas desde MetodoPago
    }
}
