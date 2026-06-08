using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Application.Profiles
{
    public class ComboProfile : Profile
    {
        public ComboProfile()
        {
            // Mapeo del listado
            CreateMap<Combo, ComboDTO>()
                .ForMember(dest => dest.CategoriaNombre,
                           opt => opt.MapFrom(src => src.Categoria != null
                                                     ? src.Categoria.Nombre
                                                     : "Sin categoría"))
                .ReverseMap();

            // Mapeo del detalle
            CreateMap<Combo, ComboDetalleDTO>()
                .ForMember(dest => dest.CategoriaNombre,
                           opt => opt.MapFrom(src => src.Categoria != null
                                                     ? src.Categoria.Nombre
                                                     : "Sin categoría"))
                .ForMember(dest => dest.Productos,
                           opt => opt.MapFrom(src => src.ComboProducto));

            // Mapeo de la línea intermedia ComboProducto → ComboProductoDTO
            CreateMap<ComboProducto, ComboProductoDTO>()
                .ForMember(dest => dest.NombreProducto,
                           opt => opt.MapFrom(src => src.Producto.Nombre));
        }
    }
}