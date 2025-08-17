namespace ComeCat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 👉 Registramos los controladores
            builder.Services.AddControllers();

            // 👉 Registramos el HttpClientFactory (para poder inyectar HttpClient en los controladores)
            builder.Services.AddHttpClient();

            // Swagger (documentación de la API)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
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
