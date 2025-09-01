using ComeCat.Entites.Models;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;

namespace ComeCat.Services
{
    public class MqttService : IHostedService
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;
        private readonly HttpClient _httpClient;

        // Inyectamos HttpClient directamente
        public MqttService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("mqtt", 1883) // Nombre del servicio MQTT en docker
                .Build();

            _mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("Conectado a MQTT broker");

                await _mqttClient.SubscribeAsync("gatos/dispensador");
                Console.WriteLine("Suscrito al tópico: gatos/dispensador");
            };

            _mqttClient.DisconnectedAsync += async e =>
            {
                Console.WriteLine("Desconectado de MQTT, reintentando en 5s...");
                await Task.Delay(5000);
                try
                {
                    await _mqttClient.ConnectAsync(_mqttOptions);
                }
                catch
                {
                    Console.WriteLine("Error reconectando al broker");
                }
            };

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                try
                {
                    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                    Console.WriteLine($"Mensaje MQTT recibido en {e.ApplicationMessage.Topic}: {payload}");

                    var data = JsonSerializer.Deserialize<RegistroDispensador>(payload, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (data != null)
                    {
                        await GuardarEnDb(data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al procesar mensaje MQTT: {ex.Message}");
                }
            };
        }

        private async Task GuardarEnDb(RegistroDispensador registro)
        {
            try
            {
                // Usamos directamente el HttpClient inyectado
                var json = JsonSerializer.Serialize(new
                {
                    Proximidad = registro.Proximidad,
                    ServoActivo = registro.ServoActivo,
                    FechaDispensacion = registro.FechaDispensacion,
                    HoraDispensacion = registro.HoraDispensacion
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("db/dispensadores", content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Registro guardado en DB correctamente");
                }
                else
                {
                    Console.WriteLine($"Error al guardar en DB: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al guardar en DB: {ex.Message}");
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!_mqttClient.IsConnected)
            {
                try
                {
                    await _mqttClient.ConnectAsync(_mqttOptions, cancellationToken);
                }
                catch
                {
                    Console.WriteLine("Intento de conexión fallido, reintentando en 5s...");
                    await Task.Delay(5000);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }
        }
    }
}
