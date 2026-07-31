namespace SchemaDriftDetector.Core
{
    public static class SeverityRules
    {
        private static readonly Dictionary<ChangeType, Severity> Rules = new()
        {
            [ChangeType.FieldRemoved] = Severity.Breaking,
            [ChangeType.FieldAdded] = Severity.Safe,
            [ChangeType.TypeChanged] = Severity.Breaking,
            [ChangeType.BecameOptional] = Severity.Breaking,
            [ChangeType.BecameNullable] = Severity.Safe,
        };

        public static Severity Resolve(ChangeType changeType)
        {
            if (!Rules.TryGetValue(changeType, out var severity))
                throw new ArgumentOutOfRangeException(
                    nameof(changeType),
                    changeType,
                    "No severity rule defined for this ChangeType.");

            return severity;
        }
    }
}