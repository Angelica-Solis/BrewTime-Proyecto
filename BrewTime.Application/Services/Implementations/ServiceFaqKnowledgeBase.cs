using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UglyToad.PdfPig;

namespace BrewTime.Application.Services.Implementations
{
    // Lee el PDF de preguntas frecuentes (FAQ) y cachea su contenido en memoria
    // para que el chatbot lo use como unica fuente de conocimiento.
    // Se registra como Singleton, por lo que el PDF solo se procesa una vez.
    public class ServiceFaqKnowledgeBase : IServiceFaqKnowledgeBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ChatbotFaqSettings _settings;
        private readonly ILogger<ServiceFaqKnowledgeBase> _logger;

        private static string? _contenidoCacheado;
        private static readonly SemaphoreSlim _semaforo = new(1, 1);


        public ServiceFaqKnowledgeBase(
            IWebHostEnvironment hostingEnvironment,
            IOptions<ChatbotFaqSettings> settings,
            ILogger<ServiceFaqKnowledgeBase> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _settings = settings.Value;
            _logger = logger;
        }


        public async Task<string> ObtenerContenidoAsync()
        {
            // Si ya se leyo el PDF anteriormente, se devuelve directamente del cache.
            if (_contenidoCacheado != null)
            {
                return _contenidoCacheado;
            }

            await _semaforo.WaitAsync();

            try
            {
                // Doble verificacion, por si otra peticion ya lo cargo
                // mientras se esperaba el semaforo.
                if (_contenidoCacheado != null)
                {
                    return _contenidoCacheado;
                }

                string rutaCompleta = Path.Combine(
                    _hostingEnvironment.ContentRootPath,
                    _settings.RutaPdf);

                if (!File.Exists(rutaCompleta))
                {
                    _logger.LogWarning(
                        "No se encontro el PDF de preguntas frecuentes en la ruta: {Ruta}",
                        rutaCompleta);

                    _contenidoCacheado = string.Empty;
                    return _contenidoCacheado;
                }

                var textoCompleto = new StringBuilder();

                using (PdfDocument documento = PdfDocument.Open(rutaCompleta))
                {
                    foreach (var pagina in documento.GetPages())
                    {
                        textoCompleto.AppendLine(pagina.Text);
                    }
                }

                _contenidoCacheado = textoCompleto.ToString();

                return _contenidoCacheado;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al leer el PDF de preguntas frecuentes del chatbot.");

                // Si falla la lectura, no se rompe el chatbot: simplemente
                // se queda sin fuente de conocimiento y siempre respondera
                // con el mensaje de "no encontrado".
                _contenidoCacheado = string.Empty;
                return _contenidoCacheado;
            }
            finally
            {
                _semaforo.Release();
            }
        }
    }
}
