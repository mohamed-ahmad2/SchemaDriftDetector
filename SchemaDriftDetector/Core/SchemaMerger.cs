namespace SchemaDriftDetector.Core
{
    public static class SchemaMerger
    {
        private const int MaxDepth = 50;

        public static SchemaNode Merge(SchemaNode baseline, SchemaNode newSample)
            => Merge(baseline, newSample, depth: 0);

        private static SchemaNode Merge(SchemaNode baseline, SchemaNode newSample, int depth)
        {
            if (depth > MaxDepth)
                return new SchemaNode { Type = DataType.Unknown };

            var merged = new SchemaNode
            {
                Type = baseline.Type == DataType.Unknown ? newSample.Type : baseline.Type,
                IsNullable = baseline.IsNullable || newSample.IsNullable,
                IsOptional = baseline.IsOptional || newSample.IsOptional
            };

            var allKeys = new HashSet<string>(baseline.Properties.Keys);
            allKeys.UnionWith(newSample.Properties.Keys);

            foreach (var key in allKeys)
            {
                var inBaseline = baseline.Properties.TryGetValue(key, out var oldNode);
                var inSample = newSample.Properties.TryGetValue(key, out var newNode);

                if (inBaseline && inSample)
                {
                    merged.Properties[key] = Merge(oldNode!, newNode!, depth + 1);
                }
                else
                {
                    var present = (inBaseline ? oldNode : newNode)!;
                    merged.Properties[key] = CloneWithOptional(present);
                }
            }

            merged.ArrayElementType = (baseline.ArrayElementType, newSample.ArrayElementType) switch
            {
                (null, var n) => n,
                (var b, null) => b,
                (var b, var n) => Merge(b!, n!, depth + 1)
            };

            return merged;
        }

        private static SchemaNode CloneWithOptional(SchemaNode source)
        {
            var clone = new SchemaNode
            {
                Type = source.Type,
                IsNullable = source.IsNullable,
                IsOptional = true,
                ArrayElementType = source.ArrayElementType
            };

            foreach (var (key, value) in source.Properties)
                clone.Properties[key] = value;

            return clone;
        }
    }
}