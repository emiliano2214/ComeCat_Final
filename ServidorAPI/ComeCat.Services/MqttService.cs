using MQTTnet;
using MQTTnet.Client;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace ComeCat.Services
{
    public class MqttService : IHostedService
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;

        public MqttService()
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("mqtt", 1883) // Usar nombre del servicio en docker
                .Build();

            // Evento al conectar
            _mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("Conectado a MQTT broker");

                await _mqttClient.SubscribeAsync("gatos/dispensador");
                Console.WriteLine("Suscrito al tópico: gatos/dispensador");
            };

            // Evento al desconectar
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

            // Evento al recibir un mensaje
            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                Console.WriteLine($"Mensaje MQTT recibido en {e.ApplicationMessage.Topic}: {payload}");
                return Task.CompletedTask;
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_mqttClient.IsConnected)
            {
                await _mqttClient.ConnectAsync(_mqttOptions, cancellationToken);
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
