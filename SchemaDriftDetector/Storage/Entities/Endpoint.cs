namespace SchemaDriftDetector.Storage.Entities
{
    public class Endpoint
    {
        public Guid Id { get; set; }
        public string RouteTemplate { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string ApiVersion { get; set; } = string.Empty;
        public DateTime FirstSeenAt { get; set; }
        public DateTime LastSeenAt { get; set; }
        public bool IsDeprecated { get; set; } = false;
        public SchemaBaseline? SchemaBaseline { get; set; }
        public ICollection<SchemaVersion> SchemaVersions { get; set; } = new List<SchemaVersion>();
        public ICollection<PendingDrift> PendingDrifts { get; set; } = new List<PendingDrift>();
        public ICollection<DriftAlert> DriftAlerts { get; set; } = new List<DriftAlert>();
    }
}