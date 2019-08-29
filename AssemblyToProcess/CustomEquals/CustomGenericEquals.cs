[Equals]
public struct CustomGenericEquals<T>
{
    [IgnoreDuringEquals]
    public T Prop { get; set; }

    [CustomEqualsInternal]
    bool CustomEquals(CustomGenericEquals<T> other)
    {
        return Equals(Prop, other.Prop);
    }

    [CustomGetHashCode]
    int CustomGetHashCode()
    {
        return 42;
    }

    public static bool operator ==(CustomGenericEquals<T> left, CustomGenericEquals<T> right) => Operator.Weave(left, right);
    public static bool operator !=(CustomGenericEquals<T> left, CustomGenericEquals<T> right) => Operator.Weave(left, right);
}