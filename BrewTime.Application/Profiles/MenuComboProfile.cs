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
    public class MenuComboProfile : Profile
    {
        public MenuComboProfile()
        {
            CreateMap<MenuCombo, MenuComboDTO>()
                .ForMember(
                    dest => dest.Menu,
                    opt => opt.Ignore());

            CreateMap<MenuComboDTO, MenuCombo>()
                .ForMember(
                    dest => dest.Menu,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Combo,
                    opt => opt.Ignore());
        }
    }
}
