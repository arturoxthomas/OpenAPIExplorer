using OpenAPIExplorer.Domain;
namespace OpenAPIExplorer.Application;
public interface IWeatherService { Task<WeatherResult> GetAsync(string city, CancellationToken ct = default); }
public interface ICurrencyService {
 Task<CurrencyResult> ConvertAsync(string from, string to, decimal amount, CancellationToken ct = default);
 Task<IReadOnlyList<CurrencyOption>> GetCurrenciesAsync(CancellationToken ct = default);
}
public record CurrencyOption(string Code, string Name);
public interface ICountryService {
 Task<IReadOnlyList<CountryResult>> SearchAsync(string name, CancellationToken ct = default);
 Task<IReadOnlyList<string>> GetNamesAsync(CancellationToken ct = default);
}
public interface IAstronomyService { Task<AstronomyResult> GetTodayAsync(CancellationToken ct = default); }