namespace SchemaDriftDetector.Core
{
    public static class SchemaDiffer
    {
        private const int MaxDepth = 50;

        public static List<SchemaDifference> Compare(SchemaNode oldSchema, SchemaNode newSchema)
        {
            var differences = new List<SchemaDifference>();
            CompareNodes(oldSchema, newSchema, path: "$", depth: 0, differences);
            return differences;
        }

        private static void CompareNodes(SchemaNode oldNode, SchemaNode newNode, string path, int depth, List<SchemaDifference> differences)
        {
            if (depth > MaxDepth)
                return;

            if (oldNode.Type != newNode.Type && oldNode.Type != DataType.Unknown && newNode.Type != DataType.Unknown)
            {
                differences.Add(new SchemaDifference(path, ChangeType.TypeChanged, SeverityRules.Resolve(ChangeType.TypeChanged)));
                return;
            }

            if (!oldNode.IsNullable && newNode.IsNullable)
            {
                differences.Add(new SchemaDifference(path, ChangeType.BecameNullable, SeverityRules.Resolve(ChangeType.BecameNullable)));
            }

            if (!oldNode.IsOptional && newNode.IsOptional)
            {
                differences.Add(new SchemaDifference(path, ChangeType.BecameOptional, SeverityRules.Resolve(ChangeType.BecameOptional)));
            }

            if (oldNode.Type == DataType.Object || newNode.Type == DataType.Object)
            {
                CompareProperties(oldNode, newNode, path, depth, differences);
            }

            if (oldNode.Type == DataType.Array || newNode.Type == DataType.Array)
            {
                CompareArrayElement(oldNode, newNode, path, depth, differences);
            }
        }

        private static void CompareProperties(SchemaNode oldNode, SchemaNode newNode, string path, int depth, List<SchemaDifference> differences)
        {
            var allKeys = new HashSet<string>(oldNode.Properties.Keys);
            allKeys.UnionWith(newNode.Properties.Keys);

            foreach (var key in allKeys)
            {
                var childPath = $"{path}.{key}";
                var inOld = oldNode.Properties.TryGetValue(key, out var oldChild);
                var inNew = newNode.Properties.TryGetValue(key, out var newChild);

                if (inOld && !inNew)
                {
                    differences.Add(new SchemaDifference(childPath, ChangeType.FieldRemoved, SeverityRules.Resolve(ChangeType.FieldRemoved)));
                }
                else if (!inOld && inNew)
                {
                    differences.Add(new SchemaDifference(childPath, ChangeType.FieldAdded, SeverityRules.Resolve(ChangeType.FieldAdded)));
                }
                else if (inOld && inNew)
                {
                    CompareNodes(oldChild!, newChild!, childPath, depth + 1, differences);
                }
            }
        }

        private static void CompareArrayElement(SchemaNode oldNode, SchemaNode newNode, string path, int depth, List<SchemaDifference> differences)
        {
            var oldElement = oldNode.ArrayElementType;
            var newElement = newNode.ArrayElementType;

            if (oldElement is null && newElement is null)
                return;
            if (oldElement is null || newElement is null)
                return;

            CompareNodes(oldElement, newElement, $"{path}[]", depth + 1, differences);
        }
    }
}