using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BrewTime.Application.DTOs;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Application.Profiles
{
    public class CarritoProfile : Profile
    {
        public CarritoProfile()
        {
            CreateMap<Carrito, CarritoItemDTO>()
                .ForMember(dest => dest.Nombre,
                    opt => opt.MapFrom(src =>
                        src.Producto != null
                            ? src.Producto.Nombre
                            : src.Combo!.Nombre))

                .ForMember(dest => dest.PrecioUnitario,
                    opt => opt.MapFrom(src =>
                        src.Producto != null
                            ? src.Producto.Precio
                            : src.Combo!.PrecioEspecial))
                .ForMember(dest => dest.Imagen,
                    opt => opt.MapFrom(src =>
                        src.Producto != null
                        ? src.Producto.ProductoImagen
                        .OrderByDescending(i => i.EsPrincipal)
                        .Select(i => i.RutaImagen)
                        .FirstOrDefault()
                        : src.Combo != null
                        ? src.Combo.RutaImagen
                        : null));
        }
    }
}