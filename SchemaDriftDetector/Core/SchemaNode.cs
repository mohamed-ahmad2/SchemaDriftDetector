namespace SchemaDriftDetector.Core
{
    public enum DataType
    {
        String,
        Number,
        Boolean,
        Object,
        Array,
        Unknown
    }

    public class SchemaNode
    {
        public DataType Type { get; set; } = DataType.Unknown;
        public bool IsOptional { get; set; }
        public bool IsNullable { get; set; }
        public Dictionary<string, SchemaNode> Properties { get; set; } = new();
        public SchemaNode? ArrayElementType { get; set; }
    }
}