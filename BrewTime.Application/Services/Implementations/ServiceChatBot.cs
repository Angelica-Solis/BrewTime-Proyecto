using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.Configuration;
using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceChatBot : IServiceChatBot
    {
        private readonly IOpenRouterService _openRouterService;
        private readonly IServiceFaqKnowledgeBase _faqKnowledgeBase;

        // Token que le pedimos a la IA que devuelva EXACTAMENTE cuando la
        // respuesta no se pueda contestar con la informacion del PDF.
        // Se intercepta en el codigo y se reemplaza por un mensaje fijo,
        // para no depender 100% de que la IA redacte bien el "no se".
        private const string TOKEN_SIN_RESPUESTA = "NO_ENCONTRADO_EN_FAQ";

        private const string MENSAJE_SIN_RESPUESTA =
            "No encontré esa información en nuestras preguntas frecuentes. " +
            "Te recomendamos llamarnos o visitarnos directamente en BrewTime " +
            "para que podamos ayudarte con más detalle!!";


        public ServiceChatBot(
            IOpenRouterService openRouterService,
            IServiceFaqKnowledgeBase faqKnowledgeBase)
        {
            _openRouterService = openRouterService;
            _faqKnowledgeBase = faqKnowledgeBase;
        }


        public async Task<ChatResponseDTO> SendMessageAsync(ChatRequestDTO request)
        {

            string contenidoFaq =
                await _faqKnowledgeBase.ObtenerContenidoAsync();


            // Si el PDF no se pudo leer o esta vacio, no tiene sentido
            // llamar a la IA: no hay fuente de conocimiento disponible.
            if (string.IsNullOrWhiteSpace(contenidoFaq))
            {
                return new ChatResponseDTO
                {
                    Response = MENSAJE_SIN_RESPUESTA
                };
            }


            string systemPrompt = ConstruirSystemPrompt(contenidoFaq);


            string respuestaIA =
                await _openRouterService.SendMessageAsync(
                    systemPrompt,
                    request.Message);


            // Si la IA indica que no encontro la respuesta en el documento,
            // se reemplaza por el mensaje fijo de contacto/visita.
            bool noEncontroRespuesta =
                string.IsNullOrWhiteSpace(respuestaIA) ||
                respuestaIA.Contains(
                    TOKEN_SIN_RESPUESTA,
                    StringComparison.OrdinalIgnoreCase);

            string respuestaFinal =
                noEncontroRespuesta
                    ? MENSAJE_SIN_RESPUESTA
                    : respuestaIA;


            return new ChatResponseDTO
            {
                Response = respuestaFinal
            };
        }


        private string ConstruirSystemPrompt(string contenidoFaq)
        {
            return
                $"""
                Eres el asistente virtual de la cafetería BrewTime.

                Tu ÚNICA fuente de información es el documento de preguntas
                frecuentes (FAQ) que aparece más abajo, delimitado por
                "### INICIO FAQ ###" y "### FIN FAQ ###".

                Reglas obligatorias:
                1. Responde SOLO con información que esté explícita o
                   implícitamente contenida en el documento FAQ.
                2. No inventes horarios, precios, direcciones ni ningún otro
                   dato que no esté en el documento.
                3. Si la pregunta del usuario NO se puede responder con la
                   información del documento, responde ÚNICAMENTE con el
                   texto exacto: {TOKEN_SIN_RESPUESTA}
                   (sin explicaciones, sin saludos, sin texto adicional).
                4. Responde de forma breve, clara y amigable, en español.

                ### INICIO FAQ ###
                {contenidoFaq}
                ### FIN FAQ ###
                """;
        }

    }
}
