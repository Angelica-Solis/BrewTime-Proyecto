using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BrewTime.Application.Services.Implementations
{
    public class ServiceRutaEntrega : IServiceRutaEntrega
    {
        private readonly HttpClient _httpClient;
        private readonly OpenRouteServiceSettings _settings;

        public ServiceRutaEntrega(HttpClient httpClient, IOptions<OpenRouteServiceSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<EntregaRutaDTO> CalcularRutaAsync(string direccion)
        {
            //validar direccion
            if (string.IsNullOrWhiteSpace(direccion))
            {
                throw new InvalidOperationException("Debe ingresar una dirección de entrega");
            }

            //validar API Key
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                throw new InvalidOperationException("No se encuentra configurada la API Key de OpenRouteService");
            }

            //validar ubicación de BrewTime
            if (_settings.OrigenLatitud == 0 &&
                _settings.OrigenLongitud == 0)
            {
                throw new InvalidOperationException("No se encuentra configurada la ubicación del establecimiento");
            }

            //convertir la direccion del cliente en latitud y longitud
            var ubicacion = await ObtenerCoordenadasAsync(direccion);

            //calcular distancia y tiempo desde BrewTime hasta el cliente
            var ruta = await ObtenerRutaAsync( ubicacion.Longitud,ubicacion.Latitud);

            //calcular el costo adicional segun la distancia
            decimal costoPorDistancia = CalcularCostoPorDistancia(ruta.DistanciaKilometro);

            //devolver resultado
            return new EntregaRutaDTO
            {
                DireccionEncontrada = direccion,

                LatitudDestino = ubicacion.Latitud,

                LongitudDestino = ubicacion.Longitud,

                DistanciaKilometro = ruta.DistanciaKilometro,

                TiempoEstimado = ruta.TiempoEstimado,

                CostoPorDistancia = costoPorDistancia
            };
        }

        private async Task<(double Latitud, double Longitud, string DireccionEncontrada)>ObtenerCoordenadasAsync(string direccion)
        {
            string direccionCompleta = $"{direccion}, Costa Rica";

            string url =
                $"{_settings.GeocodingBaseUrl.TrimEnd('/')}/search" +
                $"?text={Uri.EscapeDataString(direccionCompleta)}" +
                "&size=1&boundary.country=CR";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("Authorization", _settings.ApiKey);

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("No fue posible consultar la dirección con el servicio de mapas");

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument document = JsonDocument.Parse(json);

            JsonElement root = document.RootElement;

            if (!root.TryGetProperty("features", out JsonElement features) ||
                features.GetArrayLength() == 0)
            {
                throw new InvalidOperationException("No se encontró la dirección indicada");
            }

            JsonElement primerResultado = features[0];
            JsonElement coordenadas = primerResultado
                .GetProperty("geometry")
                .GetProperty("coordinates");

            //OpenRouteService devuelve [longitud, latitud]
            double longitud = coordenadas[0].GetDouble();
            double latitud = coordenadas[1].GetDouble();

            string direccionEncontrada = direccion;

            if (primerResultado.TryGetProperty("properties", out JsonElement properties) &&
                properties.TryGetProperty("label", out JsonElement label))
            {
                direccionEncontrada = label.GetString() ?? direccion;
            }

            return (latitud, longitud, direccionEncontrada);
        }


        private async Task<(double DistanciaKilometro, int TiempoEstimado)>ObtenerRutaAsync(double longitudDestino, double latitudDestino)
        {
            string url =
                $"{_settings.DirectionsBaseUrl.TrimEnd('/')}/v2/directions/driving-car/json";

            var body = new
            {
                coordinates = new[]
                {
            new[] { _settings.OrigenLongitud, _settings.OrigenLatitud },
            new[] { longitudDestino, latitudDestino }
        }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.TryAddWithoutValidation("Authorization", _settings.ApiKey);
            request.Content = JsonContent.Create(body);

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException(
                    "No fue posible calcular la ruta de entrega.");

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument document = JsonDocument.Parse(json);

            JsonElement routes = document.RootElement.GetProperty("routes");

            if (routes.GetArrayLength() == 0)
                throw new InvalidOperationException(
                    "No se encontró una ruta hacia la dirección indicada.");

            JsonElement resumen = routes[0].GetProperty("summary");

            double distanciaMetros = resumen.GetProperty("distance").GetDouble();
            double tiempoSegundos = resumen.GetProperty("duration").GetDouble();

            double distanciaKilometro =
                Math.Round(distanciaMetros / 1000.0, 2);

            int tiempoEstimado =
                (int)Math.Ceiling(tiempoSegundos / 60.0);

            return (distanciaKilometro, tiempoEstimado);
        }

        private static decimal CalcularCostoPorDistancia(double distanciaKilometro)
        {
            //los promeros 3km es gratis
            if (distanciaKilometro <= 3)
                return 0m;

            //después de 3km se cobran ₡500 por cada bloque de hasta 5 km adicionales
            double kilometrosExtra = distanciaKilometro - 3;

            return (decimal)Math.Ceiling(kilometrosExtra / 5) * 500m;

            throw new InvalidOperationException("La dirección se encuentra fuera de nuestra zona de entrega");
        }

        public async Task<List<DireccionSugeridaDTO>> BuscarDireccionesAsync(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto) || texto.Length < 3)
                return new List<DireccionSugeridaDTO>();

            string url =
                $"{_settings.GeocodingBaseUrl.TrimEnd('/')}/autocomplete" +
                $"?text={Uri.EscapeDataString(texto)}" +
                "&size=5&boundary.country=CR";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("Authorization", _settings.ApiKey);

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new List<DireccionSugeridaDTO>();

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument document = JsonDocument.Parse(json);

            var resultado = new List<DireccionSugeridaDTO>();

            if (!document.RootElement.TryGetProperty("features", out JsonElement features))
                return resultado;

            foreach (JsonElement feature in features.EnumerateArray())
            {
                if (!feature.TryGetProperty("properties", out JsonElement properties))
                    continue;

                if (!properties.TryGetProperty("label", out JsonElement label))
                    continue;

                string? direccion = label.GetString();

                if (!string.IsNullOrWhiteSpace(direccion))
                {
                    resultado.Add(new DireccionSugeridaDTO
                    {
                        Direccion = direccion
                    });
                }
            }

            return resultado;
        }
    }
}
