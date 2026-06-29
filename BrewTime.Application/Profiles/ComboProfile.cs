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

            // Mapeo del formulario Combo → ComboFormDTO
            CreateMap<Combo, ComboFormDTO>()
            .ForMember(dest => dest.CategoriaID,
                opt => opt.MapFrom(src => src.CategoriaId))
            .ForMember(dest => dest.ProductosSeleccionados,
                opt => opt.MapFrom(src => src.ComboProducto));

            // Mapeo inverso ComboFormDTO → Combo
            CreateMap<ComboFormDTO, Combo>()
            .ForMember(dest => dest.CategoriaId,
                       opt => opt.MapFrom(src => src.CategoriaID))
            .ForMember(dest => dest.ComboProducto, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.MenuCombo, opt => opt.Ignore())
            .ForMember(dest => dest.PedidoDetalle, opt => opt.Ignore());

            CreateMap<ComboProducto, ComboProductoFormDTO>()
            .ForMember(dest => dest.ProductoID,
                opt => opt.MapFrom(src => src.ProductoId))
            .ForMember(dest => dest.NombreProducto,
                opt => opt.MapFrom(src => src.Producto.Nombre))
            .ForMember(dest => dest.Cantidad,
                opt => opt.MapFrom(src => src.Cantidad))
            .ForMember(dest => dest.Seleccionado,
                opt => opt.MapFrom(src => true));
        }
    }
}