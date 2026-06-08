using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Application.Profiles
{
    public class ProductoProfile : Profile
    {
        public ProductoProfile()
        {
            // Mapeo del listado (ya existía)
            CreateMap<Producto, ProductoDTO>()
                .ForMember(dest => dest.CategoriaNombre,
                           opt => opt.MapFrom(src => src.Categoria != null
                                                     ? src.Categoria.Nombre
                                                     : "Sin categoría"))
                .ReverseMap();

            // Mapeo del detalle (nuevo)
            CreateMap<Producto, ProductoDetalleDTO>()
    .ForMember(dest => dest.CategoriaNombre,
               opt => opt.MapFrom(src => src.Categoria != null
                                         ? src.Categoria.Nombre
                                         : "Sin categoría"))
    .ForMember(dest => dest.Ingredientes,
               opt => opt.MapFrom(src => src.Ingrediente        // directo
                                            .Select(i => i.Nombre)
                                            .ToList()))
    .ForMember(dest => dest.ImagenPrincipal,
               opt => opt.MapFrom(src => src.ProductoImagen     // sin la 's'
                                            .Where(i => i.EsPrincipal == true)
                                            .Select(i => i.RutaImagen)
                                            .FirstOrDefault()));
        }
    }
}