using generator_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace generator_web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlertController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("check")]
        public async Task<IActionResult> CheckConditions([FromBody] generator_data data)
        {
            try
            {
                if (data == null)
                    return BadRequest("Missed Data");

                var alerts = new List<AlertInfo>();

                // 1. Vérifier si l'essence est à moins de 25%
                if (data.YakitSeviyesi < 25)
                {
                    alerts.Add(new AlertInfo
                    {
                        Type = "FUEL_LOW",
                        Message = $"Alert: Fuel low ({data.YakitSeviyesi}%)",
                        Severity = "WARNING",
                        Timestamp = DateTime.Now
                    });
                }

                // 2. Vérifier s'il n'y a ni électricité ni générateur
                if (data.SebekeHz==0 && data.GenUretilenGuc==0)
                {
                    alerts.Add(new AlertInfo
                    {
                        Type = "NO_POWER",
                        Message = "Alert: No power source available (neither electricity nor generator)",
                        Severity = "CRITICAL",
                        Timestamp = DateTime.Now
                    });
                }

                // 3. Vérifier s'il y a les deux (électricité ET générateur) - situation anormale
                if (data.SebekeHz > 0 && data.GenUretilenGuc > 0)
                {
                    alerts.Add(new AlertInfo
                    {
                        Type = "DUAL_POWER",
                        Message = "Alert: Abnormal situation - Electricity and generator are operating simultaneously",
                        Severity = "WARNING",
                        Timestamp = DateTime.Now
                    });
                }

                // Sauvegarder les données avec vérification des alertes
                _context.generator_datas.Add(data);
                await _context.SaveChangesAsync();

                // Retourner le résultat avec les alertes
                return Ok(new
                {
                    message = alerts.Count > 0 ? "Data added with alerts" : "Data added without alerts",
                    data_saved = true,
                    alerts = alerts,
                    alert_count = alerts.Count,
                    timestamp = data.timestamp
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error when verification",
                    error = ex.Message
                });
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetCurrentStatus()
        {
            try
            {
                // Récupérer les dernières données
                var latestData = await Task.FromResult(
                    _context.generator_datas
                        .OrderByDescending(x => x.timestamp)
                        .FirstOrDefault()
                );

                if (latestData == null)
                    return NotFound("Aucune donnée trouvée");

                var currentAlerts = new List<AlertInfo>();

                // Vérifier les mêmes conditions sur les dernières données
                if (latestData.YakitSeviyesi < 25)
                {
                    currentAlerts.Add(new AlertInfo
                    {
                        Type = "FUEL_LOW",
                        Message = $"Niveau d'essence faible ({latestData.YakitSeviyesi}%)",
                        Severity = "WARNING",
                        Timestamp = DateTime.Now
                    });
                }

                if (latestData.SebekeHz ==0 && latestData.GenUretilenGuc == 0) 
                {
                    currentAlerts.Add(new AlertInfo
                    {
                        Type = "NO_POWER",
                        Message = "No alimentation source found",
                        Severity = "CRITICAL",
                        Timestamp = DateTime.Now
                    });
                }

                if (latestData.SebekeHz >0 && latestData.GenUretilenGuc > 0)
                {
                    currentAlerts.Add(new AlertInfo
                    {
                        Type = "DUAL_POWER",
                        Message = "Anormal Situation - twice alimentation",
                        Severity = "WARNING",
                        Timestamp = DateTime.Now
                    });
                }

                return Ok(new
                {
                    current_data = latestData,
                    status = currentAlerts.Count > 0 ? "ALERT" : "NORMAL",
                    alerts = currentAlerts,
                    alert_count = currentAlerts.Count,
                    check_timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error when status verification",
                    error = ex.Message
                });
            }
        }

        [HttpGet("active")]
        public IActionResult GetActiveAlerts()
        {
            try
            {
                var alerts = _context.Alerts
                    .Where(a => a.IsActive)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList(); // Utiliser ToList() au lieu de ToListAsync()

                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Alertes alınırken hata oluştu",
                    error = ex.Message
                });
            }
        }

            //[HttpGet("history")]
            //public async Task<IActionResult> GetAlertHistory(int limit = 10)
            //{
            //    try
            //    {
            //        // Récupérer les dernières données pour analyser l'historique
            //        var recentData = await Task.FromResult(
            //            _context.generator_datas
            //                .OrderByDescending(x => x.timestamp)
            //                .Take(limit)
            //                .ToList()
            //        );

            //        var alertHistory = new List<object>();

            //        foreach (var data in recentData)
            //        {
            //            var alerts = new List<AlertInfo>();

            //            if (data.YakitSeviyesi < 25)
            //            {
            //                alerts.Add(new AlertInfo
            //                {
            //                    Type = "FUEL_LOW",
            //                    Message = $"Niveau d'essence faible ({data.YakitSeviyesi}%)",
            //                    Severity = "WARNING",
            //                    Timestamp = data.DateTime
            //                });
            //            }

            //            if (data.SebekeHz == 0  && data.GenUretilenGuc == 0)
            //            {
            //                alerts.Add(new AlertInfo
            //                {
            //                    Type = "NO_POWER",
            //                    Message = "No alimentation source founded",
            //                    Severity = "CRITICAL",
            //                    Timestamp = data.DateTime
            //                });
            //            }

            //            if (data.SebekeHz >0 && data.GenUretilenGuc > 0)
            //            {
            //                alerts.Add(new AlertInfo
            //                {
            //                    Type = "DUAL_POWER",
            //                    Message = "Double alimentation anormale",
            //                    Severity = "WARNING",
            //                    Timestamp = data.DateTime
            //                });
            //            }

            //            alertHistory.Add(new
            //            {
            //                timestamp = data.timestamp,
            //                data = data,
            //                alerts = alerts,
            //                status = alerts.Count > 0 ? "ALERT" : "NORMAL"
            //            });
            //        }

            //        return Ok(new
            //        {
            //            history = alertHistory,
            //            total_records = alertHistory.Count,
            //            records_with_alerts = alertHistory.Count(h => ((dynamic)h).status == "ALERT")
            //        });
            //    }
            //    catch (Exception ex)
            //    {
            //        return StatusCode(500, new
            //        {
            //            message = "Erreur lors de la récupération de l'historique",
            //            error = ex.Message
            //        });
            //    }
            //}
    }

       
    // Classe pour structurer les informations d'alerte
    public class AlertInfo
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; } // WARNING, CRITICAL, INFO
        public DateTime DateTime { get; set; }

        public DateTime Timestamp { get; set; }
    }


}