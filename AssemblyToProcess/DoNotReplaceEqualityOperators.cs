using System;

[Equals(DoNotAddEqualityOperators = true)]
public class DoNotReplaceEqualityOperators
{
    public DoNotReplaceEqualityOperators()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }

    public static bool operator ==(DoNotReplaceEqualityOperators left, DoNotReplaceEqualityOperators right) => true;
    public static bool operator !=(DoNotReplaceEqualityOperators left, DoNotReplaceEqualityOperators right) => true;
}