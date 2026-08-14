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
        private const int MaxDepth = 50;

        public DataType Type { get; set; } = DataType.Unknown;
        public bool IsOptional { get; set; }
        public bool IsNullable { get; set; }
        public Dictionary<string, SchemaNode> Properties { get; set; } = new();
        public SchemaNode? ArrayElementType { get; set; }

        public bool StructurallyEquals(SchemaNode? other)
        {
            if (other is null)
                return false;

            return StructurallyEquals(this, other, depth: 0);
        }

        private static bool StructurallyEquals(SchemaNode a, SchemaNode b, int depth)
        {
            if (depth > MaxDepth)
                return false;

            if (a.Type != b.Type)
                return false;

            if (a.IsOptional != b.IsOptional)
                return false;

            if (a.IsNullable != b.IsNullable)
                return false;

            if (a.Type == DataType.Object)
            {
                if (a.Properties.Count != b.Properties.Count)
                    return false;

                foreach (var (key, aChild) in a.Properties)
                {
                    if (!b.Properties.TryGetValue(key, out var bChild))
                        return false;

                    if (!StructurallyEquals(aChild, bChild!, depth + 1))
                        return false;
                }
            }

            if (a.Type == DataType.Array)
            {
                var aElement = a.ArrayElementType;
                var bElement = b.ArrayElementType;

                if (aElement is null && bElement is null)
                {
                    // both empty arrays observed so far -> considered equal
                }
                else if (aElement is null || bElement is null)
                {
                    return false;
                }
                else if (!StructurallyEquals(aElement, bElement, depth + 1))
                {
                    return false;
                }
            }

            return true;
        }
    }
}