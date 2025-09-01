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

        private int _distancia;
        private bool _servo;

        public MqttService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("mqtt", 1883)
                .Build();

            _mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("Conectado a MQTT broker");

                await _mqttClient.SubscribeAsync("dispensador/distancia");
                await _mqttClient.SubscribeAsync("dispensador/servo");

                Console.WriteLine("Suscrito a tópicos: dispensador/distancia, dispensador/servo");
            };

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                try
                {
                    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                    Console.WriteLine($"Mensaje MQTT recibido en {e.ApplicationMessage.Topic}: {payload}");

                    if (e.ApplicationMessage.Topic == "dispensador/distancia")
                    {
                        if (int.TryParse(payload, out var distancia))
                            _distancia = distancia;
                    }
                    else if (e.ApplicationMessage.Topic == "dispensador/servo")
                    {
                        if (bool.TryParse(payload, out var servo))
                            _servo = servo;
                    }

                    // Creamos objeto con valores actuales
                    var registro = new RegistroDispensador
                    {
                        Proximidad = _distancia,
                        ServoActivo = _servo ? 1 : 0,
                        FechaDispensacion = DateTime.Now.ToString("yyyy-MM-dd"),
                        HoraDispensacion = DateTime.Now.ToString("HH:mm:ss")
                    };

                    await GuardarEnApi(registro);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al procesar mensaje MQTT: {ex.Message}");
                }
            };
        }

        private async Task GuardarEnApi(RegistroDispensador registro)
        {
            try
            {
                var json = JsonSerializer.Serialize(registro);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // 👉 Ahora apuntamos a nuestra propia API
                var response = await _httpClient.PostAsync("http://localhost:5000/api/dispensador", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Registro enviado a API correctamente");
                }
                else
                {
                    Console.WriteLine($"Error al enviar a API: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al enviar a API: {ex.Message}");
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
