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
using BrewTime.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BrewTime.Infraestructure.Configuration;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using BrewTime.Web;

var builder = WebApplication.CreateBuilder(args);


// configuracion de serilog 
var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .WriteTo.Console(LogEventLevel.Information)
    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(@"Logs\Info-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(@"Logs\Debug-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(@"Logs\Warning-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(@"Logs\Error-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File(@"Logs\Fatal-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
    .CreateLogger();

builder.Host.UseSerilog(logger);

//  configuracion de servicios 

// localizacion e idiomas 
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
// login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Login/AccessDenied";

        options.Cookie.Name = "BrewTimeAuth";

        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        options.SlidingExpiration = true;
    });

builder.Services
    .AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.AddSingleton<SharedResource>();


builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "es-ES", "en-US" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

// dependencias del repository
builder.Services.AddTransient<IRepositoryProducto, RepositoryProducto>();
builder.Services.AddTransient<IRepositoryCategoria, RepositoryCategoria>();
builder.Services.AddTransient<IRepositoryCombo, RepositoryCombo>();
builder.Services.AddTransient<IRepositoryMenu, RepositoryMenu>();
builder.Services.AddTransient<IRepositoryMenuDiaSemana, RepositoryMenuDiaSemana>();
builder.Services.AddTransient<IRepositoryMenuProducto, RepositoryMenuProducto>();
builder.Services.AddTransient<IRepositoryMenuCombo, RepositoryMenuCombo>();
builder.Services.AddTransient<IRepositoryProcesoPreparacion, RepositoryProcesoPreparacion>();
builder.Services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();
builder.Services.AddTransient<IRepositoryEstacionCocina, RepositoryEstacionCocina>();
builder.Services.AddTransient<IRepositoryIngrediente, RepositoryIngrediente>();
builder.Services.AddTransient<IRepositoryCarrito, RepositoryCarrito>();
builder.Services.AddTransient<IRepositoryPedido, RepositoryPedido>();


// dependencias del services 
builder.Services.AddTransient<IServiceProducto, ServiceProducto>();
builder.Services.AddTransient<IServiceCategoria, ServiceCategoria>();
builder.Services.AddTransient<IServiceCombo, ServiceCombo>();
builder.Services.AddTransient<IServiceEstacionCocina, ServiceEstacionCocina>();
builder.Services.AddTransient<IServiceMenu, ServiceMenu>();
builder.Services.AddTransient<IServiceMenuDiaSemana, ServiceMenuDiaSemana>();
builder.Services.AddTransient<IServiceMenuProducto, ServiceMenuProducto>();
builder.Services.AddTransient<IServiceMenuCombo, ServiceMenuCombo>();
builder.Services.AddTransient<IServiceProcesoPreparacion, ServiceProcesoPreparacion>();
builder.Services.AddTransient<IServiceUsuario, ServiceUsuario>();
builder.Services.AddTransient<IServiceIngrediente, ServiceIngrediente>();
builder.Services.AddTransient<IServiceAutenticacion, ServiceAutenticacion>();
builder.Services.AddTransient<IServiceCarrito, ServiceCarrito>();
builder.Services.AddTransient<IServicePedido, ServicePedido>();

// CHATBOT 
builder.Services.Configure<OpenRouterSettings>(builder.Configuration.GetSection("OpenRouter"));
builder.Services.Configure<ChatbotFaqSettings>(builder.Configuration.GetSection("ChatbotFaq"));
builder.Services.AddScoped<IServiceChatBot, ServiceChatBot>();
builder.Services.AddScoped<IOpenRouterService, OpenRouterService>();
// Singleton porque el PDF se lee y se cachea una sola vez en memoria.
builder.Services.AddSingleton<IServiceFaqKnowledgeBase, ServiceFaqKnowledgeBase>();

// automapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<ProductoProfile>();
    config.AddProfile<ComboProfile>();
    config.AddProfile<MenuProfile>();
    config.AddProfile<MenuDiaSemanaProfile>();
    config.AddProfile<MenuProductoProfile>();
    config.AddProfile<MenuComboProfile>();
    config.AddProfile<ProcesoPreparacionProfile>();
    config.AddProfile<UsuarioProfile>();
    config.AddProfile<EstacionCocinaProfile>();
    config.AddProfile<CarritoProfile>();
    config.AddProfile<PedidoProfile>();

});

// bd
builder.Services.AddDbContext<BrewTimeContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDataBase"));
    if (builder.Environment.IsDevelopment()) options.EnableSensitiveDataLogging();
});

// construccion de la aplicacion 
var app = builder.Build();

// configuracion pipeline

// middleware de idiomas (Debe ir antes del enrutamiento)
var localizationOptions =
    app.Services.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>();

app.UseRequestLocalization(localizationOptions.Value);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseMiddleware<ErrorHandlingMiddleware>();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// ejecucion
app.Run();