namespace SchemaDriftDetector.Storage.Entities
{
    public class SchemaBaseline
    {
        public Guid Id { get; set; }

        public Guid EndpointId { get; set; }
        public Endpoint Endpoint { get; set; } = null!;

        public string SchemaJson { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}