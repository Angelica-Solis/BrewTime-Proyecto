using BrewTime.Application.Profiles;
using BrewTime.Application.Services.Implementations;
using BrewTime.Application.Services.Interfaces;
using BrewTime.Infraestructure.Data;
using BrewTime.Infraestructure.Repository.Implemetations;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Serilog;
using System.Text;
using BrewTime.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Configurar D.I. //Repository 
builder.Services.AddTransient<IRepositoryProducto, RepositoryProducto>();
builder.Services.AddTransient<IRepositoryCombo, RepositoryCombo>();
<<<<<<< HEAD
builder.Services.AddTransient<IRepositoryMenu, RepositoryMenu>();
builder.Services.AddTransient<IRepositoryMenuDiaSemana, RepositoryMenuDiaSemana>();
builder.Services.AddTransient<IRepositoryMenuProducto, RepositoryMenuProducto>();
builder.Services.AddTransient<IRepositoryMenuCombo, RepositoryMenuCombo>();
=======
builder.Services.AddTransient<IRepositoryProcesoPreparacion, RepositoryProcesoPreparacion>();
>>>>>>> Angelica

//Services 
builder.Services.AddTransient<IServiceProducto, ServiceProducto>();
builder.Services.AddTransient<IServiceCombo, ServiceCombo>();
<<<<<<< HEAD
builder.Services.AddTransient<IServiceMenu, ServiceMenu>();
builder.Services.AddTransient<IServiceMenuDiaSemana, ServiceMenuDiaSemana>();
builder.Services.AddTransient<IServiceMenuProducto, ServiceMenuProducto>();
builder.Services.AddTransient<IServiceMenuCombo, ServiceMenuCombo>();
=======
builder.Services.AddTransient<IServiceProcesoPreparacion, ServiceProcesoPreparacion>();
>>>>>>> Angelica

//Configurar Automapper 
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<ProductoProfile>();
    config.AddProfile<ComboProfile>();
<<<<<<< HEAD
    config.AddProfile<MenuProfile>();
    config.AddProfile<MenuDiaSemanaProfile>();
    config.AddProfile<MenuProductoProfile>();
    config.AddProfile<MenuComboProfile>();
=======
    config.AddProfile<ProcesoPreparacionProfile>();
>>>>>>> Angelica
});

// Configuar Conexión a la Base de Datos SQL 
builder.Services.AddDbContext<BrewTimeContext>(options => {
    // it read appsettings.json file 
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDataBase"));

    if (builder.Environment.IsDevelopment()) options.EnableSensitiveDataLogging();
});


//***********************
//Configuracion Serilog
// Logger. P.E. Verbose = muestra SQl Statement
var logger = new LoggerConfiguration()
                    // Limitar la informacion de depuracion
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .Enrich.FromLogContext()
                    // Log LogEventLevel.Verbose muestra mucha informacion, pero no es necesaria solo para el proceso de depuracion
                    .WriteTo.Console(LogEventLevel.Information)
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(@"Logs\Info-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(@"Logs\Debug-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(@"Logs\Warning-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(@"Logs\Error-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File(@"Logs\Fatal-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .CreateLogger();

builder.Host.UseSerilog(logger);
//***************************



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Error control Middleware 
    app.UseMiddleware<ErrorHandlingMiddleware>();
}


//Activar soporte a la solicitud de registro con SERILOG 
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// Activar Antiforgery  
app.UseAntiforgery();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
