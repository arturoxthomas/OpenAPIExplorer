using OpenAPIExplorer.Domain;
namespace OpenAPIExplorer.Application;
public interface IWeatherService { Task<WeatherResult> GetAsync(string city, CancellationToken ct = default); }
public interface ICurrencyService { Task<CurrencyResult> ConvertAsync(string from, string to, decimal amount, CancellationToken ct = default); }
public interface ICountryService { Task<IReadOnlyList<CountryResult>> SearchAsync(string name, CancellationToken ct = default); }
public interface IAstronomyService { Task<AstronomyResult> GetTodayAsync(CancellationToken ct = default); }