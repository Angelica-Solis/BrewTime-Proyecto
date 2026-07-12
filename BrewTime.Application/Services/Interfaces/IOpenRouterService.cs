using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IOpenRouterService
    {
        Task<string> SendMessageAsync(string message);
    }
}
