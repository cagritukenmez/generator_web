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
            // 1. Veriyi al
            if (data == null)
                return BadRequest("Veri boş olamaz");

            // 2. Veritabanına ekle
            _context.generator_datas.Add(data);

            // 3. Kaydet
            await _context.SaveChangesAsync();

            // 4. Cevap dön
            return Ok(new { message = "Veri başarıyla eklendi", id = data.Id });
        }
    }

}
