
using Microsoft.AspNetCore.SignalR;
namespace generator_web.Hubs
{
    public class GeneratorHub : Hub
    {
        public async Task SendGeneratorCommand(string command)
        {
            // Komut işleme logici (şimdilik sadece log)
            Console.WriteLine($"Komut alındı: {command}");

            await Clients.All.SendAsync("CommandResponse", new
            {
                message = $"{command} komutu alındı ve işlendi",
                success = true
            });
        }

        public async Task RequestGeneratorData()
        {
            // Test için mock data
            var mockData = GetMockGeneratorData();
            await Clients.Caller.SendAsync("ReceiveGeneratorData", mockData);
        }

        private object GetMockGeneratorData()
        {
            var random = new Random();
            return new
            {
                genUretilenGuc = random.Next(50, 300),
                motorRpm = random.Next(1200, 1800),
                yakitSeviyesi = random.Next(20, 100),
                bataryaVoltaji = Math.Round(random.NextDouble() * 2 + 12, 1),
                calismaDurumu = "Çalışıyor",
                operationMode = "AUTO",
                sistemCalismaSuresi = random.Next(3600, 86400),
                sistemSagligi = "Normal",
                sebekeVoltaj_l1 = random.Next(220, 240),
                sebekeVoltaj_l2 = random.Next(220, 240),
                sebekeVoltaj_l3 = random.Next(220, 240),
                sebekeHz = Math.Round(random.NextDouble() * 2 + 49, 1),
                toplamGuc = random.Next(100, 500),
                sebekeDurumu = "Normal",
                genVoltaj_l1 = random.Next(380, 400),
                genVoltaj_l2 = random.Next(380, 400),
                genVoltaj_l3 = random.Next(380, 400),
                genHz = Math.Round(random.NextDouble() * 1 + 49.5, 1),
                genGucFaktoru = Math.Round(random.NextDouble() * 0.2 + 0.8, 2),
                motorSicaklik = random.Next(70, 90),
                yagBasinci = random.Next(60, 100)
            };
        }
    }
}
