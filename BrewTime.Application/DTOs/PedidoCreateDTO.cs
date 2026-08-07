using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public class PedidoCreateDTO
    {
        //permite saber si la vista debe mostrar o no el selector de clientes
        public bool EsClienteLogueado { get; set; }

        /*
         * cliente al que pertenece el pedido
         * para un cliente logueado se asigna automáticamente
         * para un encargado se selecciona desde una lista
         */
        public int? ClienteId { get; set; }

        [Display(Name = "Cliente")]
        public string ClienteNombre { get; set; } = string.Empty;

        [Display(Name = "Correo")]
        public string ClienteCorreo { get; set; } = string.Empty;

        //solo tendrá valor cuando el usuario logueado sea un encargado
        
        [Display(Name = "Encargado")]
        public string EncargadoNombre { get; set; } = string.Empty;

        [Display(Name = "Fecha del pedido")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Debe seleccionar un método de entrega")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un método de entrega válido")]
        [Display(Name = "Método de entrega")]
        public int? MetodoEntregaId { get; set; }

        //se utiliza únicamente para mostrar el nombre después de consultar la base de datos
        public string MetodoEntregaNombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La dirección no puede superar los 500 caracteres")]
        [Display(Name = "Dirección de entrega")]
        public string? DireccionEntrega { get; set; }

        [Display(Name = "Costo de envío")]
        public decimal CostoEnvio { get; set; }

        [Display(Name = "Estado")]
        public string EstadoNombre { get; set; } = string.Empty;

        //Líneas obtenidas del carrito
        public List<PedidoLineaCreateDTO> Detalles { get; set; } = new();

        //Listas necesarias para preparar el formulario
        public List<UsuarioDetalleDTO> ClientesDisponibles { get; set; } =  new();

        public List<MetodoEntregaDTO> MetodosEntrega { get; set; } = new();

       //Totales mostrados en formato de factura
        public decimal Subtotal => Detalles.Sum(x => x.Subtotal);

        public decimal Impuesto => Detalles.Sum(x => x.Impuesto);

        public decimal Total => Subtotal + Impuesto + CostoEnvio;
    }
}
