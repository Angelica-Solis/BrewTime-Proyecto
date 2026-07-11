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
    public class MenuDiaSemanaProfile : Profile
    {
        public MenuDiaSemanaProfile()
        {
            CreateMap<MenuDiaSemana, MenuDiaSemanaDTO>()
                .ForMember(
                    dest => dest.Menu,
                    opt => opt.Ignore());

            CreateMap<MenuDiaSemanaDTO, MenuDiaSemana>()
                .ForMember(
                    dest => dest.Menu,
                    opt => opt.Ignore());
        }
    }
}
