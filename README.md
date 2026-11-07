# OpenAPI Explorer 🌐

ASP.NET Core MVC **.NET 10** portfolio project that consumes four public APIs with a responsive Razor frontend.

## Modules
- **Weather**: Open-Meteo geocoding and current temperature/wind.
- **Currency**: Frankfurter v2 exchange rate conversion.
- **Countries**: REST Countries v3.1 search, flags, capital and population.
- **Astronomy**: NASA Astronomy Picture of the Day.

## Architecture
- `src/OpenAPIExplorer.Domain`: immutable result models
- `src/OpenAPIExplorer.Application`: service contracts
- `src/OpenAPIExplorer.Infrastructure`: typed HTTP clients and JSON parsing
- `src/OpenAPIExplorer.Web`: MVC controllers, Razor frontend and CSS

## Run
Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), then run:

```bash
dotnet run --project src/OpenAPIExplorer.Web
```

Open the localhost URL shown in the console. Or open `OpenAPIExplorer.sln` in Visual Studio and select `OpenAPIExplorer.Web` as the startup project.

**Status:** initial prototype; compilation, browser execution and external API uptime have not yet been verified. NASA and REST Countries may change public API availability. No API keys or database are required for this prototype.

**Author:** [Arturo Thomas](https://github.com/arturoxthomas)
