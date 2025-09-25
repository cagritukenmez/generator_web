using System.Diagnostics;
using generator_web.Models;
using Microsoft.AspNetCore.Mvc;

namespace generator_web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // En son veriyi çek
            var data = _context.generator_datas
                        .OrderByDescending(d => d.timestamp)
                        .FirstOrDefault();
            /*
            ViewBag.gun = data.timestamp/;
            ViewBag.saat = 50;
            ViewBag.dakika = 50;
            ViewBag.saniye = 50;
            */
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
