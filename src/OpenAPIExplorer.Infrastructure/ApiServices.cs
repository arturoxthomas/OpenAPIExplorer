using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using OpenAPIExplorer.Application;
using OpenAPIExplorer.Domain;

namespace OpenAPIExplorer.Infrastructure;

internal static class JsonHelper
{
    public static string? Text(this JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
    public static double Number(this JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.TryGetDouble(out var d) ? d : 0;
    public static long Integer(this JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.TryGetInt64(out var d) ? d : 0;
}
public sealed class WeatherService(HttpClient http) : IWeatherService
{
    public async Task<WeatherResult> GetAsync(string city, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(city) || city.Length > 80) throw new ArgumentException("Introduce una ciudad válida.");
        using var geo = await http.GetFromJsonAsync<JsonDocument>("https://geocoding-api.open-meteo.com/v1/search?name=" + Uri.EscapeDataString(city) + "&count=1&language=es", ct);
        if (geo is null || !geo.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0) throw new ArgumentException("No se encontró la ciudad.");
        var location = results[0];
        var lat = location.Number("latitude").ToString(CultureInfo.InvariantCulture);
        var lon = location.Number("longitude").ToString(CultureInfo.InvariantCulture);
        using var weather = await http.GetFromJsonAsync<JsonDocument>($"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m,wind_speed_10m,weather_code", ct);
        var current = weather!.RootElement.GetProperty("current");
        return new WeatherResult(location.Text("name") ?? city, current.Number("temperature_2m"), current.Number("wind_speed_10m"), (int)current.Number("weather_code"));
    }
}
public sealed class CurrencyService(HttpClient http) : ICurrencyService
{
    public async Task<CurrencyResult> ConvertAsync(string from, string to, decimal amount, CancellationToken ct = default)
    {
        from = from.Trim().ToUpperInvariant(); to = to.Trim().ToUpperInvariant();
        if (from.Length != 3 || to.Length != 3 || !from.All(char.IsLetter) || !to.All(char.IsLetter) || amount <= 0 || amount > 1000000000)
            throw new ArgumentException("Introduce monedas ISO válidas y un monto positivo.");
        if (from == to) return new(from, to, amount, 1, amount, DateTime.UtcNow.ToString("yyyy-MM-dd"));
        using var doc = await http.GetFromJsonAsync<JsonDocument>($"https://api.frankfurter.dev/v2/rate/{from}/{to}", ct);
        var root = doc!.RootElement;
        var rate = root.GetProperty("rate").GetDecimal();
        return new(from, to, amount, rate, decimal.Round(amount * rate, 2), root.Text("date") ?? "");
    }
}
public sealed class CountryService(HttpClient http) : ICountryService
{
    public async Task<IReadOnlyList<CountryResult>> SearchAsync(string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 80) throw new ArgumentException("Introduce un país válido.");
        using var response = await http.GetAsync("https://restcountries.com/v3.1/name/" + Uri.EscapeDataString(name) + "?fields=name,capital,population,region,flags", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return [];
        response.EnsureSuccessStatusCode();
        using var doc = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: ct);
        return doc!.RootElement.EnumerateArray().Take(12).Select(e => new CountryResult(
            e.GetProperty("name").Text("common") ?? "",
            e.TryGetProperty("capital", out var cap) && cap.ValueKind == JsonValueKind.Array && cap.GetArrayLength() > 0 ? cap[0].GetString() ?? "" : "",
            e.Integer("population"), e.Text("region") ?? "", e.GetProperty("flags").Text("png") ?? "")).ToList();
    }
}
public sealed class AstronomyService(HttpClient http) : IAstronomyService
{
    public async Task<AstronomyResult> GetTodayAsync(CancellationToken ct = default)
    {
        using var doc = await http.GetFromJsonAsync<JsonDocument>("https://science.nasa.gov/wp-json/wp/v2/apod-basic/", ct);
        var root = doc!.RootElement;
        var image = root.Text("hdurl");
        if (image is null && root.Text("media_type") == "image") image = root.Text("url");
        return new(root.Text("title") ?? "Astronomy Picture of the Day", root.Text("date") ?? "", root.Text("explanation") ?? "", image, root.Text("permalink") ?? root.Text("url"));
    }
}