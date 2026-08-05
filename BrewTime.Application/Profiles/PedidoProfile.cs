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

            CreateMap<EstadoPedido, EstadoPedidoDTO>();
        }
    }
}
