using OpenAPIExplorer.Application;
using OpenAPIExplorer.Infrastructure;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IWeatherService, WeatherService>(c => c.Timeout = TimeSpan.FromSeconds(12));
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>(c => c.Timeout = TimeSpan.FromSeconds(12));
builder.Services.AddHttpClient<ICountryService, CountryService>(c => c.Timeout = TimeSpan.FromSeconds(12));
builder.Services.AddHttpClient<IAstronomyService, AstronomyService>(c => c.Timeout = TimeSpan.FromSeconds(12));
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();