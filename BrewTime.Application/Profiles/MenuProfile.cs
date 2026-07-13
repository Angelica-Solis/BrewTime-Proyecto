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
    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            // Listado, detalle y menú disponible
            CreateMap<Menu, MenuDTO>();

            // Formulario de creación y modificación
            CreateMap<Menu, MenuFormDTO>()
                .ForMember(
                    dest => dest.ProductosSeleccionados,
                    opt => opt.MapFrom(src =>
                        src.MenuProducto
                            .Select(x => x.ProductoId)
                            .ToList()))
                .ForMember(
                    dest => dest.CombosSeleccionados,
                    opt => opt.MapFrom(src =>
                        src.MenuCombo
                            .Select(x => x.ComboId)
                            .ToList()))

                .ForMember(
                    dest => dest.DiasSeleccionados,
                    opt => opt.MapFrom(src =>
                        src.MenuDiaSemana
                            .Select(x => x.DiaSemana)
                            .ToList()));

            CreateMap<MenuFormDTO, Menu>()
                .ForMember(
                    dest => dest.MenuId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.FechaCreacion,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.HoraInicio,
                    opt => opt.MapFrom(src =>
                        src.HoraInicio!.Value))
                .ForMember(
                    dest => dest.HoraFin,
                    opt => opt.MapFrom(src =>
                        src.HoraFin!.Value))
                .ForMember(
                    dest => dest.MenuProducto,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.MenuCombo,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.MenuDiaSemana,
                    opt => opt.Ignore());
        }
    }
}
