namespace SchemaDriftDetector.Storage.Entities
{
    public class Endpoint
    {
        public Guid Id { get; set; }
        public string RouteTemplate { get; set; } = String.Empty;
        public string HttpMethod { get; set; } = String.Empty;
        public string Environment { get; set; } = String.Empty;
        public string Role { get; set; } = String.Empty;
        public string ApiVersion { get; set; } = String.Empty;
        public DateTime FirstSeenAt { get; set; } 
        public DateTime LastSeenAt { get; set; }
        public bool IsDeprecated { get; set; } = false;
    }
}
