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
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Usuario, UsuarioDTO>()
            .ForMember(
                dest => dest.NombreRol,
                opt => opt.MapFrom(src => src.Rol.Nombre)
            );

            // mapeo del detalle
            CreateMap<Usuario, UsuarioDetalleDTO>()
           .ForMember(
               dest => dest.NombreRol,
               opt => opt.MapFrom(src => src.Rol.Nombre)
           );
        }

    }
}
