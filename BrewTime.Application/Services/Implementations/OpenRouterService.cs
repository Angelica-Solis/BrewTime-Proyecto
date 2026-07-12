using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BrewTime.Application.Services.Implementations
{
    public class OpenRouterService : IOpenRouterService
    {
        private readonly OpenRouterSettings _settings;


        public OpenRouterService(
        IOptions<OpenRouterSettings> settings)
        {
            _settings = settings.Value;
        }



        public async Task<string> SendMessageAsync(string message)
        {

            using HttpClient client = new();


            client.DefaultRequestHeaders.Add(
            "Authorization",
            $"Bearer {_settings.ApiKey}"
            );


            client.DefaultRequestHeaders.Add(
            "HTTP-Referer",
            "https://localhost:5001"
            );


            client.DefaultRequestHeaders.Add(
            "X-Title",
            "BrewTime Chatbot"
            );



            var body = new
            {
                model = _settings.Model,

                messages = new[]
                {
        new
        {
            role="system",
            content=
            """
            Eres un asistente de una cafetería.
            Responde únicamente información relacionada con BrewTime.
            """
        },

        new
        {
            role="user",
            content=message
        }
    }
            };



            string json =
            JsonConvert.SerializeObject(body);



            var content =
            new StringContent(
            json,
            Encoding.UTF8,
            "application/json");



            var response =
            await client.PostAsync(
            "https://openrouter.ai/api/v1/chat/completions",
            content);



            string result =
            await response.Content.ReadAsStringAsync();


            Console.WriteLine(result);

            dynamic data =
            JsonConvert.DeserializeObject(result);



            return data.choices[0].message.content;

        }

    }

}

