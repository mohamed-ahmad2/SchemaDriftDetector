namespace SchemaDriftDetector.Core
{
    public static class SchemaDiffer
    {
        public static List<SchemaDifference> Compare(SchemaNode oldSchema, SchemaNode newSchema)
        {
            var differences = new List<SchemaDifference>();
            CompareNodes(oldSchema, newSchema, path: "$", differences);
            return differences;
        }

        private static void CompareNodes(SchemaNode oldNode, SchemaNode newNode, string path, List<SchemaDifference> differences)
        {
            if (oldNode.Type != newNode.Type && oldNode.Type != DataType.Unknown && newNode.Type != DataType.Unknown)
            {
                differences.Add(new SchemaDifference(path, ChangeType.TypeChanged, Severity.Breaking));

                return;
            }

            if (!oldNode.IsNullable && newNode.IsNullable)
            {
                differences.Add(new SchemaDifference(path, ChangeType.BecameNullable, Severity.Safe));
            }

            if (!oldNode.IsOptional && newNode.IsOptional)
            {
                differences.Add(new SchemaDifference(path, ChangeType.BecameOptional, Severity.Breaking));
            }

            if (oldNode.Type == DataType.Object || newNode.Type == DataType.Object)
            {
                CompareProperties(oldNode, newNode, path, differences);
            }

            if (oldNode.Type == DataType.Array || newNode.Type == DataType.Array)
            {
                CompareArrayElement(oldNode, newNode, path, differences);
            }
        }

        private static void CompareProperties(SchemaNode oldNode, SchemaNode newNode, string path, List<SchemaDifference> differences)
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
                    differences.Add(new SchemaDifference(childPath, ChangeType.FieldRemoved, Severity.Breaking));
                }
                else if (!inOld && inNew)
                {
                    differences.Add(new SchemaDifference(childPath, ChangeType.FieldAdded, Severity.Safe));
                }
                else if (inOld && inNew)
                {
                    CompareNodes(oldChild!, newChild!, childPath, differences);
                }
            }
        }

        private static void CompareArrayElement(SchemaNode oldNode, SchemaNode newNode, string path, List<SchemaDifference> differences)
        {
            var oldElement = oldNode.ArrayElementType;
            var newElement = newNode.ArrayElementType;

            if (oldElement is null && newElement is null)
                return;
            if (oldElement is null || newElement is null)
                return;

            CompareNodes(oldElement, newElement, $"{path}[]", differences);
        }
    }
}