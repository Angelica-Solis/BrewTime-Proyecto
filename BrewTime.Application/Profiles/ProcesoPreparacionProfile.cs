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
    public class ProcesoPreparacionProfile : Profile
    {
        public ProcesoPreparacionProfile()
        {
            CreateMap<ProcesoPreparacion, ProcesoPreparacionDTO>();
        }
    }
}
