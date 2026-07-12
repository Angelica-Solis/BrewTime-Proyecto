using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.DTOs;

namespace BrewTime.Application.Services.Interfaces
{
    public interface IServiceChatBot
    {
        Task<ChatResponseDTO> SendMessageAsync(ChatRequestDTO request);
    }
}
