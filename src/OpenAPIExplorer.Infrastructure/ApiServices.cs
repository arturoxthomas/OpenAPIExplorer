using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using OpenAPIExplorer.Application;
using OpenAPIExplorer.Domain;

namespace OpenAPIExplorer.Infrastructure;
internal static class JsonHelper {
 public static string? Text(this JsonElement e,string key) => e.ValueKind==JsonValueKind.Object && e.TryGetProperty(key,out var v) && v.ValueKind==JsonValueKind.String ? v.GetString() : null;
 public static double Number(this JsonElement e,string key) => e.TryGetProperty(key,out var v) && v.TryGetDouble(out var d) ? d : 0;
 public static long Integer(this JsonElement e,string key) => e.TryGetProperty(key,out var v) && v.TryGetInt64(out var d) ? d : 0;
}
public sealed class WeatherService(HttpClient http):IWeatherService {
 public async Task<WeatherResult> GetAsync(string city,CancellationToken ct=default) {
  if(string.IsNullOrWhiteSpace(city)||city.Length>80)throw new ArgumentException("Introduce una ciudad válida.");
  using var geo=await http.GetFromJsonAsync<JsonDocument>("https://geocoding-api.open-meteo.com/v1/search?name="+Uri.EscapeDataString(city)+"&count=1&language=es",ct);
  if(geo is null||!geo.RootElement.TryGetProperty("results",out var results)||results.GetArrayLength()==0)throw new ArgumentException("No se encontró la ciudad.");
  var loc=results[0];var lat=loc.Number("latitude").ToString(CultureInfo.InvariantCulture);var lon=loc.Number("longitude").ToString(CultureInfo.InvariantCulture);
  using var weather=await http.GetFromJsonAsync<JsonDocument>($"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m,wind_speed_10m,weather_code",ct);
  var current=weather!.RootElement.GetProperty("current");
  return new(loc.Text("name")??city,current.Number("temperature_2m"),current.Number("wind_speed_10m"),(int)current.Number("weather_code"));
 }
}
public sealed class CurrencyService(HttpClient http):ICurrencyService {
 public async Task<IReadOnlyList<CurrencyOption>> GetCurrenciesAsync(CancellationToken ct=default) {
  using var doc=await http.GetFromJsonAsync<JsonDocument>("https://api.frankfurter.dev/v2/currencies",ct);
  return doc!.RootElement.EnumerateArray().Where(x=>x.Text("type")=="currency" && !string.IsNullOrWhiteSpace(x.Text("iso_code"))).Select(x=>new CurrencyOption(x.Text("iso_code")!,x.Text("name")??x.Text("iso_code")!)).OrderBy(x=>x.Code).ToList();
 }
 public async Task<CurrencyResult> ConvertAsync(string from,string to,decimal amount,CancellationToken ct=default) {
  from=(from??"").Trim().ToUpperInvariant();to=(to??"").Trim().ToUpperInvariant();
  if(from.Length!=3||to.Length!=3||!from.All(char.IsLetter)||!to.All(char.IsLetter)||amount<=0||amount>1000000000)throw new ArgumentException("Selecciona monedas válidas y un monto positivo.");
  if(from==to)return new(from,to,amount,1,amount,DateTime.UtcNow.ToString("yyyy-MM-dd"));
  using var doc=await http.GetFromJsonAsync<JsonDocument>($"https://api.frankfurter.dev/v2/rate/{from}/{to}",ct);
  var root=doc!.RootElement;var rate=root.GetProperty("rate").GetDecimal();
  return new(from,to,amount,rate,decimal.Round(amount*rate,2),root.Text("date")??"");
 }
}
public sealed class CountryService(HttpClient http):ICountryService {
 private const string Url="https://pw-d20261017a29.plainwork-apps.workers.dev/v3.1/all?fields=name,capital,population,region,flags";
 private async Task<IReadOnlyList<CountryResult>> AllAsync(CancellationToken ct) {
  using var doc=await http.GetFromJsonAsync<JsonDocument>(Url,ct);
  return doc!.RootElement.EnumerateArray().Select(e=>new CountryResult(
   e.GetProperty("name").Text("common")??"",
   e.TryGetProperty("capital",out var cap)&&cap.ValueKind==JsonValueKind.Array&&cap.GetArrayLength()>0?cap[0].GetString()??"":"",
   e.Integer("population"),e.Text("region")??"",e.GetProperty("flags").Text("png")??"")).OrderBy(x=>x.Name).ToList();
 }
 public async Task<IReadOnlyList<string>> GetNamesAsync(CancellationToken ct=default) => (await AllAsync(ct)).Select(x=>x.Name).ToList();
 public async Task<IReadOnlyList<CountryResult>> SearchAsync(string name,CancellationToken ct=default) {
  if(string.IsNullOrWhiteSpace(name)||name.Length>100)throw new ArgumentException("Selecciona un país válido.");
  return (await AllAsync(ct)).Where(x=>string.Equals(x.Name,name,StringComparison.OrdinalIgnoreCase)).ToList();
 }
}
public sealed class AstronomyService(HttpClient http):IAstronomyService {
 public async Task<AstronomyResult> GetTodayAsync(CancellationToken ct=default) {
  using var doc=await http.GetFromJsonAsync<JsonDocument>("https://science.nasa.gov/wp-json/wp/v2/apod-basic/?api_key=DEMO_KEY",ct);
  var root=doc!.RootElement;
  if(root.ValueKind==JsonValueKind.Array)root=root[0];
  var isImage=root.Text("media_type")=="image";
  var image=isImage?(root.Text("hdurl")??root.Text("url")):null;
  return new(root.Text("title")??"Astronomy Picture of the Day",root.Text("date")??"",root.Text("explanation")??"",image,root.Text("permalink")??root.Text("url"));
 }
}