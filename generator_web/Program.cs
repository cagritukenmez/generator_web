
using generator_web.Models; // AppDbContext burada tanımlı olmalı
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔗 Connection string'i appsettings.json'dan al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.WebHost.UseUrls("http://0.0.0.0:5156");

// 🧠 DbContext'i DI (Dependency Injection)'a ekle
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddControllers();
// MVC servislerini ekle
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
var app = builder.Build();

// 🧱 HTTP Pipeline ayarları
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Varsayılan route tanımı
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
