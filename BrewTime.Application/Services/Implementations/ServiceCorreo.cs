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

        public async Task EnviarFacturaPdfAsync(string destino, string asunto, string cuerpoHtml, byte[] archivo, string nombreArchivo)
        {
            var host = _configuration["Smtp:Host"];
            var port = int.Parse(_configuration["Smtp:Port"]!);
            var user = _configuration["Smtp:User"];
            var password = _configuration["Smtp:Password"];

            using var mensaje = new MailMessage();
            mensaje.From = new MailAddress(user!, "BrewTime");
            mensaje.To.Add(destino);
            mensaje.Subject = asunto;
            mensaje.Body = cuerpoHtml;
            mensaje.IsBodyHtml = true;

            using var stream = new MemoryStream(archivo);
            mensaje.Attachments.Add(new Attachment(stream, nombreArchivo, "application/pdf"));

            using var smtp = new SmtpClient(host!, port);
            smtp.Credentials = new NetworkCredential(user, password);
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(mensaje);
        }
    }
}