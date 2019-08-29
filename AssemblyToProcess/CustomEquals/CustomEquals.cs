[Equals]
public class CustomEquals
{
    [IgnoreDuringEquals]
    public int X { get; set; }

    [CustomEqualsInternal]
    bool CustomEqualsMethod(CustomEquals other)
    {
        return X == 1 && other.X == 2 || X == 2 && other.X == 1;
    }

    [CustomGetHashCode]
    int CustomGetHashCode()
    {
        return 42;
    }

    public static bool operator ==(CustomEquals left, CustomEquals right) => Operator.Weave(left, right);
    public static bool operator !=(CustomEquals left, CustomEquals right) => Operator.Weave(left, right);
}