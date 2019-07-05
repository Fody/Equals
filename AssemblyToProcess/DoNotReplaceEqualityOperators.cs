[Equals(DoNotAddEqualityOperators = true)]
public class DoNotReplaceEqualityOperators
{
    public static bool operator ==(DoNotReplaceEqualityOperators left, DoNotReplaceEqualityOperators right) => true;
    public static bool operator !=(DoNotReplaceEqualityOperators left, DoNotReplaceEqualityOperators right) => true;
}