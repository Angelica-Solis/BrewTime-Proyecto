using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.Configuration;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using OpenAI;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceChatBot : IServiceChatBot
    {
        private readonly IOpenRouterService _openRouterService;


        public ServiceChatBot(
            IOpenRouterService openRouterService)
        {
            _openRouterService = openRouterService;
        }


        public async Task<ChatResponseDTO> SendMessageAsync(ChatRequestDTO request)
        {

            Dictionary<string, string> preguntas = new()
            {
                {
                    "horario",
                    "Nuestro horario es de lunes a viernes de 7:00 AM a 5:00 PM."
                },

                {
                    "ubicacion",
                    "Estamos ubicados en San José, Costa Rica."
                },

                {
                    "menu",
                    "Contamos con café, bebidas frías, bubble tea y productos preparados."
                },

                {
                    "contacto",
                    "Puedes contactarnos por WhatsApp o Facebook."
                }
            };


            string mensaje =
                request.Message.ToLower();


            foreach (var pregunta in preguntas)
            {
                if (mensaje.Contains(pregunta.Key))
                {
                    return new ChatResponseDTO
                    {
                        Response = pregunta.Value
                    };
                }
            }


            //Si no encuentra palabra clave manda a IA

            string respuesta =
                await _openRouterService.SendMessageAsync(
                    request.Message);


            return new ChatResponseDTO
            {
                Response = respuesta
            };
        }

    }
}
