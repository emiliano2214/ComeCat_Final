namespace ComeCat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Registramos los controladores
            builder.Services.AddControllers();

            // Registramos el HttpClient apuntando al servicio Flask "db"
            builder.Services.AddHttpClient("DispensadorApi", client =>
            {
                client.BaseAddress = new Uri("http://db:5002/");
            });

            // Registramos MqttService
            builder.Services.AddHostedService<ComeCat.Services.MqttService>();

            // Swagger (documentación de la API)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
