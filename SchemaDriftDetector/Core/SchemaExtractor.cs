using System.Text.Json;

namespace SchemaDriftDetector.Core
{
    public static class SchemaExtractor
    {
        private const int MaxArraySamples = 5;

        public static SchemaNode? Extract(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return ExtractNode(doc.RootElement);
            }
            catch (JsonException)
            {
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        private static SchemaNode ExtractNode(JsonElement jsonElement)
        {
            var node = new SchemaNode();

            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Object:
                    node.Type = DataType.Object;
                    foreach (var prop in jsonElement.EnumerateObject())
                        node.Properties[prop.Name] = ExtractNode(prop.Value);
                    break;

                case JsonValueKind.Array:
                    node.Type = DataType.Array;
                    node.ArrayElementType = ExtractArrayElementType(jsonElement);
                    break;

                case JsonValueKind.String:
                    node.Type = DataType.String;
                    break;

                case JsonValueKind.Number:
                    node.Type = DataType.Number;
                    break;

                case JsonValueKind.True or JsonValueKind.False:
                    node.Type = DataType.Boolean;
                    break;

                case JsonValueKind.Null:
                    node.Type = DataType.Unknown;
                    node.IsNullable = true;
                    break;
            }

            return node;
        }

        private static SchemaNode? ExtractArrayElementType(JsonElement arrayElement)
        {
            SchemaNode? merged = null;
            var sampledCount = 0;

            foreach (var item in arrayElement.EnumerateArray())
            {
                if (sampledCount >= MaxArraySamples)
                    break;

                var itemNode = ExtractNode(item);
                merged = merged == null ? itemNode : SchemaMerger.Merge(merged, itemNode);
                sampledCount++;
            }

            return merged;
        }
    }
}