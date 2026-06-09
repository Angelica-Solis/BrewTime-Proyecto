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
    public class MenuProductoProfile : Profile
    {
        public MenuProductoProfile()
        {
            CreateMap<MenuProductoDTO, MenuProducto>().ReverseMap();
        }
    }
}
