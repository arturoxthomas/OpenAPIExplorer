namespace OpenAPIExplorer.Domain;
public record WeatherResult(string City, double Temperature, double WindSpeed, int WeatherCode);
public record CurrencyResult(string From, string To, decimal Amount, decimal Rate, decimal Converted, string Date);
public record CountryResult(string Name, string Capital, long Population, string Region, string Flag);
public record AstronomyResult(string Title, string Date, string Explanation, string? ImageUrl, string? SourceUrl);