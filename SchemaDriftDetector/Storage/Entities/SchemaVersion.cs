namespace SchemaDriftDetector.Storage.Entities
{
    public class SchemaVersion
    {
        public Guid Id { get; set; }

        public Guid EndpointId { get; set; }
        public Endpoint Endpoint { get; set; } = new Endpoint();

        public Guid? DeployId { get; set; } = null;
        public Deploy? Deploy { get; set; } = null;

        public string SchemaJson { get; set; } = string.Empty;
        public string ChangeReason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}