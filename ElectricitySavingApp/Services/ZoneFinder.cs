namespace ElectricitySavingApp.Services;

using NetTopologySuite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

public sealed class ZoneFinder : IZoneFinder
{
	// Client used to download the published zone-boundary GeoJSON file.
	private readonly HttpClient httpClient;
	// Factory that creates points with the WGS84 coordinate system used by GeoJSON.
	private readonly GeometryFactory geometryFactory;
	// Lazy loading ensures the boundaries are downloaded only on first use.
	private readonly Lazy<Task<FeatureCollection>> zones;

	public ZoneFinder(HttpClient httpClient)
	{
		this.httpClient = httpClient;
		geometryFactory =
			NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
		zones = new Lazy<Task<FeatureCollection>>(LoadZonesAsync);
	}

	public async Task<string> GetZone(
		double latitude,
		double longitude,
		CancellationToken cancellationToken = default)
	{
		// Reject impossible geographic coordinates before doing geometry work.
		if (latitude is < -90 or > 90)
			throw new ArgumentOutOfRangeException(nameof(latitude));

		if (longitude is < -180 or > 180)
			throw new ArgumentOutOfRangeException(nameof(longitude));

		// Reuse the cached boundary collection for all future requests.
		FeatureCollection zoneFeatures = await zones.Value.WaitAsync(cancellationToken);

		// GeoJSON coordinates are ordered as longitude, latitude.
		Point point = geometryFactory.CreatePoint(
			new Coordinate(longitude, latitude));

		// Find the polygon that covers the user's coordinate.
		foreach (Feature feature in zoneFeatures)
		{
			if (!feature.Geometry.Covers(point))
				continue;

			// The published file stores the bidding-zone identifier in "zone".
			string? zone = feature.Attributes.Exists("zone")
				? feature.Attributes["zone"]?.ToString()
				: feature.Attributes.Exists("id")
					? feature.Attributes["id"]?.ToString()
					: null;

			if (!string.IsNullOrWhiteSpace(zone))
			{
				// Electricity Spot names Swedish zones SE_1 through SE_4,
				// while the price and carbon APIs use SE1 through SE4.
				return zone.Replace("_", "", StringComparison.Ordinal);
			}
		}

		throw new KeyNotFoundException(
			"No electricity zone contains the supplied coordinates.");
	}

	private async Task<FeatureCollection> LoadZonesAsync()
	{
		// Download the boundary data and parse it into NetTopologySuite features.
		string json = await httpClient.GetStringAsync(
			"data/zones.geojson");

		GeoJsonReader reader = new();
		return reader.Read<FeatureCollection>(json);
	}
}
