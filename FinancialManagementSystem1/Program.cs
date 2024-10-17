using FinancialManagementSystem1.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

builder.Services.AddDbContext<FinancialDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"));
});

// Configuraci�n de la sesi�n
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Duraci�n de la sesi�n (30 minutos)
    options.Cookie.HttpOnly = true; // Evitar acceso desde scripts del lado del cliente
    options.Cookie.IsEssential = true; // Marcar la cookie como esencial
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


// Habilitar el uso de sesiones
app.UseSession(); // A�adir este middleware antes de los endpoints

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
