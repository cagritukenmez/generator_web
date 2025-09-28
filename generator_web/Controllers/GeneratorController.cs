using generator_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;

namespace generator_web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneratorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _espBaseUrl;

        public GeneratorController(AppDbContext context,IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _espBaseUrl = configuration.GetValue<string>("Esp:BaseUrl") ?? "http://10.82.134.173:5156";
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] generator_data data)
        {
            try
            {
                // 1. Veriyi kontrol et
                if (data == null)
                    return BadRequest("Veri boş olamaz");

                // 3. Veritabanına ekle
                _context.generator_datas.Add(data);

                // 4. Kaydet
                await _context.SaveChangesAsync();

                // 5. Başarılı cevap dön
                return Ok(new
                {
                    message = "Veri başarıyla eklendi",
                    timestamp = data.timestamp
                });
            }
            catch (Exception ex)
            {
                // Hata durumunda log ve cevap
                return StatusCode(500, new
                {
                    message = "Veri eklenirken hata oluştu",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await Task.FromResult(_context.generator_datas.ToList());
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Veri alınırken hata oluştu",
                    error = ex.Message
                });
            }
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            try
            {
                var latestData = await Task.FromResult(
                    _context.generator_datas
                        .OrderByDescending(x => x.timestamp)
                        .FirstOrDefault()
                );

                if (latestData == null)
                    return NotFound("Veri bulunamadı");

                return Ok(latestData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Veri alınırken hata oluştu",
                    error = ex.Message
                });
            }
        }

        [HttpPost("command")]
        public async Task<IActionResult> Send([FromBody] CommandDto cmd, CancellationToken cancellationToken)
        {
            if (cmd == null || string.IsNullOrWhiteSpace(cmd.Command))
                return BadRequest(new { message = "Komut boş olamaz" });

            // Hedef ESP endpoint (ESP tarafında /command POST handler olacak)
            var espUrl = $"{_espBaseUrl.TrimEnd('/')}/command";

            var client = _httpClientFactory.CreateClient();
            // İsteğe bağlı: istenirse client.Timeout = TimeSpan.FromSeconds(8);

            try
            {
                // POST JSON olarak gönder
                var response = await client.PostAsJsonAsync(espUrl, cmd, cancellationToken);

                // ESP hata döndüyse içeriği alıp devreye sok
                if (!response.IsSuccessStatusCode)
                {
                    var text = await response.Content.ReadAsStringAsync(cancellationToken);
                    return StatusCode((int)response.StatusCode, new { message = "ESP hatası", detail = text });
                }

                // Başarılıysa ESP'nin döndürdüğü JSON'u al
                object? espBody = null;
                try
                {
                    espBody = await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken);
                }
                catch
                {
                    // JSON değilse ham text al
                    espBody = await response.Content.ReadAsStringAsync(cancellationToken);
                }
                var userLog = new user_data
                {
                    //UserId = 1,//kullanıcı'nın id'si 
                    command = cmd.Command,
                    DateTime = DateTime.Now
                };
                _context.user_datas.Add(userLog);
                await _context.SaveChangesAsync();
                return Ok(new { status = "sent", command = cmd.Command, param = cmd.Param, espResponse = espBody });
            }
            catch (OperationCanceledException)
            {
                return StatusCode(504, new { message = "ESP isteği zaman aşımına uğradı" });
            }
            catch (HttpRequestException hre)
            {
                return StatusCode(502, new { message = "ESP'ye ulaşılamadı", error = hre.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        
    }

}