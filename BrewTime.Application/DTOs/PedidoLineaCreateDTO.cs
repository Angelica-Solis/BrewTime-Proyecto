using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public class PedidoLineaCreateDTO
    {
        //identifica la línea original del carrito 
        public int CarritoId { get; set; }

        public int? ProductoId { get; set; }

        public int? ComboId { get; set; }

        public bool EsCombo => ComboId.HasValue;

        public string Tipo =>
            EsCombo ? "Combo" : "Producto";

        public string Nombre { get; set; } = string.Empty;

        //valores que se muestran en la factura y el servicio vuelve a consultar el precio en la BD antes de registrar el pedido
        public decimal PrecioUnitario { get; set; }

        public int Cantidad { get; set; }

        [StringLength( 500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres")]
        [Display(Name = "Observaciones de preparación")]
        public string? Observaciones { get; set; }

        public decimal Subtotal => PrecioUnitario * Cantidad;

        public decimal Impuesto => Math.Round(Subtotal * 0.13m, 2);

        public decimal TotalLinea => Subtotal + Impuesto;
    }
}
