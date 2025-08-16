using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ComeCat.Entites.Models;

namespace ComeCat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DispensadorController : ControllerBase
    {
        private readonly HttpClient _http;

        public DispensadorController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient();
            // Importante: en docker-compose el hostname del servicio es "db"
            _http.BaseAddress = new Uri("http://db:5001/");
        }

        [HttpGet]
        public async Task<ActionResult<List<RegistroDispensador>>> GetAll()
        {
            var response = await _http.GetAsync("db/dispensadores");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Error al consultar DB");

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<RegistroDispensador>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegistroDispensador>> GetById(int id)
        {
            var response = await _http.GetAsync($"db/dispensadores/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<RegistroDispensador>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] RegistroDispensador nuevo)
        {
            var json = JsonSerializer.Serialize(nuevo);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("db/dispensadores", content);

            if (response.IsSuccessStatusCode)
                return Ok(await response.Content.ReadAsStringAsync());
            else
                return StatusCode((int)response.StatusCode, "Error al insertar en DB");
        }
    }
}
