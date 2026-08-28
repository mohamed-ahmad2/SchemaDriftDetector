using SchemaDriftDetector.Core;

namespace SchemaDriftDetector.Storage.Entities
{
    public enum PendingDriftStatus
    {
        Observing,
        Confirmed,
        Discarded
    }

    public class PendingDrift
    {
        public Guid Id { get; set; }

        public Guid EndpointId { get; set; }
        public Endpoint Endpoint { get; set; } = new Endpoint();

        public string FieldPath { get; set; } = string.Empty;
        public string ProposedSchemaJson { get; set; } = string.Empty;

        public ChangeType ChangeType { get; set; }

        public int ConsecutiveCount { get; set; } = 1;

        public PendingDriftStatus Status { get; set; } = PendingDriftStatus.Observing;

        public DateTime LastDetectedAt { get; set; }
    }
}