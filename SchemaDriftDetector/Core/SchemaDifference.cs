namespace SchemaDriftDetector.Core
{
    public enum ChangeType
    {
        BecameOptional,
        TypeChanged,
        FieldAdded,
        FieldRemoved,
        BecameNullable
    }

    public enum Severity
    {
        Breaking,
        Safe
    }

    public record SchemaDifference(string Path, ChangeType ChangeType, Severity Severity);
}
