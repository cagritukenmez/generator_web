using System.Diagnostics;
using generator_web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace generator_web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Take last datas
            var data = _context.generator_datas
                        .OrderByDescending(d => d.timestamp)
                        .FirstOrDefault();

            var currentAlerts = new List<Alert>();

            if (data != null)
            {
                // 1. Delete  same type of alerte
                var existingAlerts = _context.Alerts.Where(a => a.IsActive).ToList();
                foreach (var alert in existingAlerts)
                {
                    alert.IsActive = false;
                }

                // 2. verify alert condition and create new
                if (data.YakitSeviyesi < 25)
                {
                    var fuelAlert = new Alert
                    {
                        Type = "FUEL_LOW",
                        Message = $"Yakıt seviyesi düşük ({data.YakitSeviyesi}%)",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    currentAlerts.Add(fuelAlert);
                    _context.Alerts.Add(fuelAlert); 
                }

                if (data.SebekeHz == 0 && data.GenUretilenGuc == 0)
                {
                    var noPowerAlert = new Alert
                    {
                        Type = "NO_POWER",
                        Message = "Güç kaynağı bulunamadı (ne elektrik ne de jeneratör)",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    currentAlerts.Add(noPowerAlert);
                    _context.Alerts.Add(noPowerAlert); 
                }

                if (data.SebekeHz > 0 && data.GenUretilenGuc > 0)
                {
                    var dualPowerAlert = new Alert
                    {
                        Type = "DUAL_POWER",
                        Message = "Anormal durum - Elektrik ve jeneratör aynı anda çalışıyor",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    currentAlerts.Add(dualPowerAlert);
                    _context.Alerts.Add(dualPowerAlert); 
                }

                // 3. Save all notification
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Log error
                    Console.WriteLine($"Error when saving : {ex.Message}");
                }
            }

            ViewBag.Alerts = currentAlerts;
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