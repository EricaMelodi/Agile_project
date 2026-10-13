// Represents one feature in the electricity-zone GeoJSON document.
public class ZoneFeature
{
    // GeoJSON feature type, normally "Feature".
    public string Type { get; set; } = "";
    // Metadata such as the zone identifier and human-readable name.
    public ZoneProperties Properties { get; set; } = new();
    // Polygon geometry describing the feature's geographic boundary.
    public GeoJsonGeometry Geometry { get; set; } = new();
}

// Non-geometric metadata attached to a zone polygon.
public class ZoneProperties
{
    // Electricity bidding-zone identifier, such as SE3.
    public string Zone { get; set; } = "";
    // Human-readable name of the zone.
    public string Name { get; set; } = "";
}

// Raw GeoJSON geometry representation kept for model-level access.
public class GeoJsonGeometry
{
    // Geometry type, normally Polygon or MultiPolygon.
    public string Type { get; set; } = "";
    // Nested coordinate arrays supplied by the GeoJSON document.
    public List<object>? Coordinates { get; set; }
}