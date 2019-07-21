[Equals]
public struct CustomStructEquals
{
    [IgnoreDuringEquals]
    public int X { get; set; }

    [CustomEqualsInternal]
    bool CustomEquals(CustomStructEquals other)
    {
        return X == 1 && other.X == 2 || X == 2 && other.X == 1;
    }

    [CustomGetHashCode]
    int CustomGetHashCode()
    {
        return 42;
    }

    public static bool operator ==(CustomStructEquals left, CustomStructEquals right) => Operator.Weave();
    public static bool operator !=(CustomStructEquals left, CustomStructEquals right) => Operator.Weave();
}