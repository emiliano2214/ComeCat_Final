using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ComeCat.Entites.Models;

namespace ComeCat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly HttpClient _http;

        public TestController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient();
            // Hostname del contenedor DB donde guardaremos los resultados
            _http.BaseAddress = new Uri("http://db:5002/");
        }

        // POST api/test/resultados
        [HttpPost("resultados")]
        public async Task<ActionResult> AddResultado([FromBody] ResultadoPrueba resultado)
        {
            if (resultado == null || string.IsNullOrWhiteSpace(resultado.Mensaje))
                return BadRequest("No se recibió un mensaje válido");

            // Serializamos y enviamos al contenedor DB
            var json = JsonSerializer.Serialize(resultado);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("db/resultados", content);
            if (response.IsSuccessStatusCode)
                return Ok(await response.Content.ReadAsStringAsync());
            else
                return StatusCode((int)response.StatusCode, "Error al insertar en DB");
        }

        // GET api/test
        [HttpGet]
        public async Task<ActionResult<List<ResultadoPrueba>>> GetAll()
        {
            var response = await _http.GetAsync("db/resultados");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Error al consultar DB");

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<ResultadoPrueba>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(data);
        }

        // GET api/test/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultadoPrueba>> GetById(int id)
        {
            var response = await _http.GetAsync($"db/resultados/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ResultadoPrueba>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(data);
        }
    }
}
