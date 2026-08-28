namespace SchemaDriftDetector.Storage.Entities
{
    public class Deploy
    {
        public Guid Id { get; set; }
        public string CommitHash { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public ICollection<SchemaVersion> SchemaVersions { get; set; } = new List<SchemaVersion>();
        public ICollection<DriftAlert> DriftAlerts { get; set; } = new List<DriftAlert>();
    }
}