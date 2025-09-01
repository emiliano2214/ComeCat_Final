namespace ComeCat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Registramos los controladores
            builder.Services.AddControllers();

            // Registramos Swagger (documentación de la API)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Registramos HttpClient configurado para MqttService
            builder.Services.AddHttpClient<ComeCat.Services.MqttService>(client =>
            {
                // Este "db" es el hostname del contenedor Flask en la red de Docker
                client.BaseAddress = new Uri("http://db:5002/");
            });

            // Registramos MqttService como servicio en segundo plano
            builder.Services.AddHostedService<ComeCat.Services.MqttService>();

            // Exponemos la API en el puerto 5000 dentro del contenedor
            builder.WebHost.UseUrls("http://0.0.0.0:5000");

            var app = builder.Build();

            // Swagger siempre habilitado
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
