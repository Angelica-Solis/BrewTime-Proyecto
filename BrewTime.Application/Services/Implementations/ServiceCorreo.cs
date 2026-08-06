using System.Net;
using System.Net.Mail;
using BrewTime.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BrewTime.Infraestructure.Services
{
    public class ServiceCorreo : IServiceCorreo
    {
        private readonly IConfiguration _configuration;

        public ServiceCorreo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarAsync(string destino, string asunto, string cuerpoHtml)
        {
            var host = _configuration["Smtp:Host"];
            var puerto = int.Parse(_configuration["Smtp:Port"]!);
            var usuario = _configuration["Smtp:User"];
            var password = _configuration["Smtp:Password"];

            using var mensaje = new MailMessage
            {
                From = new MailAddress(usuario!),
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };
            mensaje.To.Add(destino);

            using var cliente = new SmtpClient(host, puerto)
            {
                Credentials = new NetworkCredential(usuario, password),
                EnableSsl = true
            };

            await cliente.SendMailAsync(mensaje);
        }
    }
}