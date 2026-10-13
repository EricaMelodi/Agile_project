# Electricity Saving App

The Electricity Saving App helps users find the electricity price and latest
grid carbon intensity for their location in Sweden.

The project contains two applications:

- `ElectricitySavingApp`: ASP.NET Core backend written in C#.
- `src`: React and TanStack frontend.

## Backend Architecture

The backend follows this flow:

```text
Address
  -> latitude and longitude
  -> electricity zone (SE1-SE4)
  -> today's price intervals
  -> current local price
  -> latest carbon intensity
```

The backend communicates with four external services:

| Service | Purpose |
| --- | --- |
| `geocode.maps.co` | Converts an address into coordinates. |
| `electricity-spot.com` | Provides GeoJSON electricity-zone boundaries. |
| `elprisetjustnu.se` | Provides Swedish electricity prices. |
| Electricity Maps | Provides latest carbon intensity. |

The browser should call the backend rather than calling these providers
directly. This keeps API keys on the server and keeps the application logic in
one place.

## Backend Structure

```text
ElectricitySavingApp/
  Controller/
    EnergyEndpoints.cs
    GeoLocationEndpoints.cs
    PriceEndpoints.cs
    CarbonIntensityEndpoints.cs
  Model/
    GeoEntry.cs
    PriceEntry.cs
    CarbonIntensityEntry.cs
    UserInput.cs
    Zone.cs
  Services/
    GeoCodeService.cs
    ZoneFinder.cs
    ElprisetJustNuService.cs
    CarbonIntensityService.cs
    UnconfiguredCarbonIntensityService.cs
```

`Program.cs` configures HTTP clients, dependency injection, CORS, and endpoint
registration.

Interfaces such as `IGeoCodeService` and `IElectricityPriceService` define
service contracts. The concrete service classes implement those contracts.

Models describe the JSON data exchanged with external APIs and with the
frontend.

## Configuration

The geocoding API key is required:

```powershell
$env:GEO_CODING_API_KEY = "your-geocoding-api-key"
```

The Electricity Maps key enables carbon-intensity data:

```powershell
$env:ELECTRICITY_MAPS_API_KEY = "your-electricity-maps-api-key"
```

Keys are read from environment variables and should not be committed to Git.
Without `ELECTRICITY_MAPS_API_KEY`, prices still work, but carbon intensity is
returned as `null`.

## Running the Backend

From the repository root:

```powershell
dotnet build .\ElectricitySavingApp\ElectricitySavingApp.csproj
dotnet run --project .\ElectricitySavingApp\ElectricitySavingApp.csproj
```

The terminal prints the address where the API is listening, commonly:

```text
http://localhost:5000
```

Use the URL printed by `dotnet run` if a different port is selected.

## Main API Endpoint

### Current energy data

```text
GET /api/current-energy?address={address}
```

Example PowerShell request:

```powershell
$address = [uri]::EscapeDataString("Drottninggatan 1, Gothenburg")
Invoke-RestMethod "http://localhost:5000/api/current-energy?address=$address"
```

This endpoint:

1. Sends the address to the geocoding service.
2. Finds the electricity zone using the returned coordinates.
3. Normalizes provider values such as `SE_3` to `SE3`.
4. Converts the current UTC time to Swedish local time.
5. Requests that day's prices for the resolved zone.
6. Selects the price interval containing the current time.
7. Requests the latest carbon intensity for the same zone.

Example response:

```json
{
  "address": "Drottninggatan 1, Gothenburg",
  "zone": "SE3",
  "latitude": 57.7046969,
  "longitude": 11.9616709,
  "localTime": "2026-10-14T14:30:00+02:00",
  "price": {
    "timeStart": "2026-10-14T14:15:00+02:00",
    "sekPerkWh": 0.85,
    "priceArea": "SE3"
  },
  "carbonIntensity": {
    "carbonIntensityValue": 42,
    "carbonIntensityUnit": "gCO2eq/kWh",
    "isEstimated": false
  }
}
```

Possible responses:

| Status | Meaning |
| --- | --- |
| `200 OK` | Energy data was found. |
| `400 Bad Request` | The address or zone is invalid or unsupported. |
| `404 Not Found` | The address or current price was not found. |

## Other API Endpoints

### Coordinates for an address

```text
GET /search?address={address}
```

Returns the latitude and longitude found by the geocoder.

### Electricity zone for an address

```text
GET /search/zone?address={address}
```

Returns the address, coordinates, and normalized electricity zone.

### Prices for a date and zone

```text
GET /api/prices?year=2026&month=10&day=14&priceArea=SE3
```

Returns all price intervals for that date and price area.

### Latest carbon intensity

```text
GET /api/carbon-intensity?priceArea=SE3
```

Returns the latest carbon-intensity measurement for the area.

## Testing with PowerShell

Test geocoding:

```powershell
$address = [uri]::EscapeDataString("Drottninggatan 1, Gothenburg")
Invoke-RestMethod "http://localhost:5000/search?address=$address"
```

Test zone detection:

```powershell
Invoke-RestMethod "http://localhost:5000/search/zone?address=$address"
```

Test prices:

```powershell
Invoke-RestMethod "http://localhost:5000/api/prices?year=2026&month=10&day=14&priceArea=SE3"
```

Test invalid input:

```powershell
Invoke-WebRequest `
  "http://localhost:5000/api/current-energy?address=" `
  -UseBasicParsing
```

The invalid request should return `400 Bad Request`.

## Frontend

Install dependencies and start the TanStack frontend:

```powershell
npm install
npm run dev
```

The frontend normally runs on:

```text
http://localhost:3000
```

The backend allows requests from this origin through its CORS policy.

The frontend uses TanStack Router. Routes are located in `src/routes`, and
`src/routeTree.gen.ts` is generated automatically.

Build the frontend with:

```powershell
npm run build
```

## Important Backend Concepts Used

This project demonstrates:

- Minimal APIs with `MapGet`.
- Interfaces and dependency injection.
- `async` and `await` for network requests.
- `HttpClient` and JSON deserialization.
- Nullable reference types such as `GeoEntry?`.
- LINQ sorting with `OrderBy`.
- Pattern matching with `is`.
- HTTP status codes such as `200`, `400`, and `404`.
- Environment variables for secrets.
- `CancellationToken` for stopping work when a request is cancelled.
- NetTopologySuite for geographic point-in-polygon calculations.
- Time-zone conversion with `TimeZoneInfo`.

## Data and Attribution

Electricity-zone boundaries are downloaded from
[electricity-spot.com](https://electricity-spot.com/data). Price data comes
from ElprisetJustNu, and carbon-intensity data comes from Electricity Maps.
