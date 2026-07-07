using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Application.Profiles
{
    public class ProductoProfile : Profile
    {
        public ProductoProfile()
        {
            // Mapeo del listado 
            CreateMap<Producto, ProductoDTO>()
                .ForMember(dest => dest.CategoriaNombre,
                           opt => opt.MapFrom(src => src.Categoria != null
                                                     ? src.Categoria.Nombre
                                                     : "Sin categoría"))
                .ReverseMap();

            // Mapeo del detalle 
            CreateMap<Producto, ProductoDetalleDTO>()
    .ForMember(dest => dest.CategoriaNombre,
               opt => opt.MapFrom(src => src.Categoria != null
                                         ? src.Categoria.Nombre
                                         : "Sin categoría"))
    .ForMember(dest => dest.Ingredientes,
               opt => opt.MapFrom(src => src.Ingrediente
                                            .Select(i => i.Nombre)
                                            .ToList()))
    .ForMember(dest => dest.Imagenes,
               opt => opt.MapFrom(src => src.ProductoImagen
                                            .Select(i => i.RutaImagen)
                                            .ToList()));

            CreateMap<ProductoImagen, ProductoImagenDTO>()
                .ForMember(dest => dest.ImagenID,
                           opt => opt.MapFrom(src => src.ImagenId));

            // Mapeo del formulario (crear/editar)
            CreateMap<Producto, ProductoFormDTO>()
                .ForMember(dest => dest.CategoriaID,
                           opt => opt.MapFrom(src => src.CategoriaId))
                .ForMember(dest => dest.ImagenesActuales,
                           opt => opt.MapFrom(src => src.ProductoImagen))
                .ForMember(dest => dest.ImagenesActualesCount,
                           opt => opt.MapFrom(src => src.ProductoImagen.Count))
                .ForMember(dest => dest.IngredientesSeleccionados,
                           opt => opt.MapFrom(src => src.Ingrediente
                                                         .Select(i => i.IngredienteId)
                                                         .ToList()))
                .ReverseMap()
                .ForMember(dest => dest.CategoriaId,
                           opt => opt.MapFrom(src => src.CategoriaID))
                .ForMember(dest => dest.ProductoImagen,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Categoria,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Ingrediente,
                           opt => opt.Ignore());
        }
    }
}