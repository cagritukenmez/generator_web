using System.Diagnostics;
using generator_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            // Récupérer les dernières données
            var data = _context.generator_datas
                        .OrderByDescending(d => d.timestamp)
                        .FirstOrDefault();

            var currentAlerts = new List<Alert>();

            if (data != null)
            {
                // Vérifier les conditions d'alerte
                if (data.YakitSeviyesi < 25)
                {
                    currentAlerts.Add(new Alert
                    {
                        Type = "FUEL_LOW",
                        Message = $"Yakýt seviyesi düþük ({data.YakitSeviyesi}%) ",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }

                if (data.SebekeHz == 0 && data.GenUretilenGuc == 0)
                {
                    currentAlerts.Add(new Alert
                    {
                        Type = "NO_POWER",
                        Message = "Güç kaynaðý bulunamadý (ne elektrik ne de jeneratör) ",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }

                if (data.SebekeHz > 0 && data.GenUretilenGuc > 0)
                {
                    currentAlerts.Add(new Alert
                    {
                        Type = "DUAL_POWER",
                        Message = "Anormal durum - Elektrik ve jeneratör ayný anda çalýþýyor ",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            // SOLUTION: Utiliser ViewBag pour passer les alertes
            ViewBag.Alerts = currentAlerts;

            // Retourner le modèle generator_data comme attendu par la vue
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