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

            // Swagger (documentación de la API)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Exponemos la API en el puerto 5007 dentro del contenedor
            builder.WebHost.UseUrls("http://0.0.0.0:5000");

            var app = builder.Build();

            // Configuración del pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
