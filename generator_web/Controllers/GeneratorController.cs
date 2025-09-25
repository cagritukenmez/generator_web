using generator_web.Models;
using Microsoft.AspNetCore.Mvc;

namespace generator_web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneratorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GeneratorController(AppDbContext context)
        {
            _context = context;
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
    }
}