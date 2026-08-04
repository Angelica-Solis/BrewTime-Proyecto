using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Infraestructure.Configuration
{
    public class ChatbotFaqSettings
    {
        // Ruta relativa (desde la raiz del proyecto Web) al PDF que contiene
        // las preguntas frecuentes que usa el chatbot como fuente de conocimiento.
        public string RutaPdf { get; set; } = "App_Data/FAQ-BrewTime.pdf";
    }
}
