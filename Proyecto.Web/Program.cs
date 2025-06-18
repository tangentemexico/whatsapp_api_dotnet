using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Mgk.Commonsx;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);
MgkFunctions.AppSettings("");

Console.WriteLine("IsLinux:" + (OperatingSystem.IsLinux() ? "Si" : "No"));
Console.WriteLine("IsWindows:" + (OperatingSystem.IsWindows() ? "Si" : "No"));
Console.WriteLine("IsMacOS:" + (OperatingSystem.IsMacOS() ? "Si" : "No"));


int IdleTimeout = 600;
string CurrentCulture = "es-MX";
string CurrentUICulture = "es-MX";

builder.Services.AddControllersWithViews()
            // Maintain property names during serialization. See:
            // https://github.com/aspnet/Announcements/issues/194
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.PropertyNamingPolicy = null
                );

CultureInfo.CurrentCulture = new CultureInfo(CurrentCulture);//
CultureInfo.CurrentUICulture = new CultureInfo(CurrentUICulture);
Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-MX");

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(IdleTimeout);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllers(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Inicio/Error");
}
app.UseStaticFiles();
app.UseCookiePolicy();


app.UseRouting();
app.UseSession();

app.UseAuthorization();


var defaultCulture = new CultureInfo("es-MX");
var supportedCultures = new[] { defaultCulture };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Index}/{id?}");

app.Run();
