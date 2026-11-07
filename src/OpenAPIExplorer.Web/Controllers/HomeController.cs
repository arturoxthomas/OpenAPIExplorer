using Microsoft.AspNetCore.Mvc;
using OpenAPIExplorer.Application;
namespace OpenAPIExplorer.Web.Controllers;
public class HomeController(IWeatherService weather, ICurrencyService currency, ICountryService countries, IAstronomyService astronomy) : Controller
{
    public IActionResult Index() => View();
    public async Task<IActionResult> Weather(string city = "Panama City")
    {
        ViewBag.City = city;
        try { return View(await weather.GetAsync(city)); }
        catch (Exception ex) { ViewBag.Error = ex is ArgumentException ? ex.Message : "No se pudo consultar el clima."; return View(); }
    }
    public async Task<IActionResult> Currency(string from = "USD", string to = "EUR", decimal amount = 100)
    {
        ViewBag.From = from; ViewBag.To = to; ViewBag.Amount = amount;
        try { return View(await currency.ConvertAsync(from, to, amount)); }
        catch (Exception ex) { ViewBag.Error = ex is ArgumentException ? ex.Message : "No se pudo consultar la tasa de cambio."; return View(); }
    }
    public async Task<IActionResult> Countries(string name = "Panama")
    {
        ViewBag.Name = name;
        try { return View(await countries.SearchAsync(name)); }
        catch (Exception ex) { ViewBag.Error = ex is ArgumentException ? ex.Message : "No se pudo consultar el país."; return View(); }
    }
    public async Task<IActionResult> Astronomy()
    {
        try { return View(await astronomy.GetTodayAsync()); }
        catch { ViewBag.Error = "No se pudo consultar la fotografía astronómica."; return View(); }
    }
    public IActionResult Error() => View("Index");
}