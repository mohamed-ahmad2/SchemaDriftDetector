using SchemaDriftDetector.Core;

namespace SchemaDriftDetector.Storage.Entities
{
    public enum DeliveryStatus
    {
        Pending,
        Sent,
        Failed
    }

    public class DriftAlert
    {
        public Guid Id { get; set; }

        public Guid EndpointId { get; set; }
        public Endpoint Endpoint { get; set; } = null!;

        public Guid? DeployId { get; set; } = null;
        public Deploy? Deploy { get; set; } = null;

        public string FieldPath { get; set; } = string.Empty;

        public Severity Severity { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.Pending;

        public int RetryCount { get; set; } = 0;
        public DateTime DetectedAt { get; set; }
    }
}