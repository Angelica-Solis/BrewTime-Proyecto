using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public class PagoPedidoDTO
    {
        [Range(1,int.MaxValue,ErrorMessage = "El pedido indicado no es válido")]
        public int PedidoId { get; set; }
         
        [Display(Name = "Total del pedido")]
        public decimal TotalPedido { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un método de pago.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un método de pago válido.")]
        [Display(Name = "Método de pago")]
        public int? MetodoPagoId { get; set; }

        //solo se utiliza para mostrar el nombre del método seleccionado
        public string MetodoPagoNombre { get; set; } = string.Empty;

        //campos para tarjeta de crédito o débito
        [StringLength(100, ErrorMessage = "El nombre del titular no puede superar los 100 caracteres")]
        [Display(Name = "Nombre del titular")]
        public string? NombreTitular { get; set; }

        [RegularExpression(@"^\d{13,19}$", ErrorMessage = "El número de tarjeta debe contener entre 13 y 19 dígitos")]
        [DataType(DataType.Password)]
        [Display(Name = "Número de tarjeta")]
        public string? NumeroTarjeta { get; set; }

        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "La fecha de vencimiento debe utilizar el formato MM/AA")]
        [Display(Name = "Fecha de vencimiento")]
        public string? FechaVencimiento { get; set; }

        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "El código de seguridad debe contener 3 o 4 dígitos")]
        [DataType(DataType.Password)]
        [Display(Name = "Código de seguridad")]
        public string? CodigoSeguridad { get; set; }

        //campo para pago en efectivo
        [Range(typeof(decimal), "0.01", "99999999.99", ErrorMessage = "El monto recibido debe ser mayor a cero")]
        [Display(Name = "Monto recibido")]
        public decimal? MontoPagado { get; set; }

        //se calcula automáticamente cuando el pago es en efectivo
      
        [Display(Name = "Vuelto")]
        public decimal? Vuelto { get; set; }

       
    }
}
