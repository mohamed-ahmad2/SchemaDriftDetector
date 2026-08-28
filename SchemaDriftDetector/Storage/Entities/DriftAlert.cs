using SchemaDriftDetector.Core;

namespace SchemaDriftDetector.Storage.Entities
{
    public class DriftAlert
    {
        public Guid Id { get; set; }

        public Guid EndpointId { get; set; }
        public Endpoint Endpoint { get; set; } = new Endpoint();

        public Guid? DeployId { get; set; } = null;
        public Deploy? Deploy { get; set; } = null;

        public string FieldPath { get; set; } = string.Empty;

        public Severity Severity { get; set; }

        public string DeliveryStatus { get; set; } = string.Empty;
        public int RetryCount { get; set; } = 0;
        public DateTime DetectedAt { get; set; }
    }
}