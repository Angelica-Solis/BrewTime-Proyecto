using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Application.Profiles
{
    public class PedidoProfile : Profile
    {
        public PedidoProfile()
        {
            CreateMap<Pedido, PedidoListDTO>()
                .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src => src.Cliente.Nombre + " " + src.Cliente.Apellidos))
                .ForMember(dest => dest.EstadoNombre, opt => opt.MapFrom(src => src.Estado.Nombre))
                .ForMember(dest => dest.MetodoEntrega, opt => opt.MapFrom(src => src.MetodoEntrega.Nombre));

            CreateMap<Pedido, PedidoDetalleDTO>()
                .ForMember(dest => dest.ClienteNombre, opt => opt.MapFrom(src => src.Cliente.Nombre + " " + src.Cliente.Apellidos))
                .ForMember(dest => dest.ClienteCorreo, opt => opt.MapFrom(src => src.Cliente.Correo))
                .ForMember(dest => dest.Encargado, opt => opt.MapFrom(src => src.Empleado != null ? src.Empleado.Nombre + " " + src.Empleado.Apellidos : "N/A"))
                .ForMember(dest => dest.MetodoEntrega, opt => opt.MapFrom(src => src.MetodoEntrega.Nombre))
                .ForMember(dest => dest.MetodoPago, opt => opt.MapFrom(src => src.MetodoPago != null ? src.MetodoPago.Nombre : "Pendiente de pago"))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.Nombre))
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => src.FechaCreacion))
                .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.PedidoDetalle));

            CreateMap<PedidoDetalle, PedidoDetalleLineaDTO>()
                .ForMember(dest => dest.Producto, opt => opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : (src.Combo != null ? src.Combo.Nombre : "N/A")))
                .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => src.PrecioUnitario))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal))
                .ForMember(dest => dest.Impuesto, opt => opt.MapFrom(src => Math.Round(src.Subtotal * 0.13m, 2)))
                .ForMember(dest => dest.Observaciones, opt => opt.MapFrom(src => src.Observaciones ?? ""));

            CreateMap<EstadoPedido, EstadoPedidoDTO>();
        }
    }
}
