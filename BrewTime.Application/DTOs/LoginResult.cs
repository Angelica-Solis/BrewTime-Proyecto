using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Application.DTOs
{
    public class LoginResult
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
