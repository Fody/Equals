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
}

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
}

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
}

[Equals]
public class CustomGetHashCode
{
    public int X { get; set; }

    [CustomGetHashCode]
    int CustomGetHashCodeMethod()
    {
        return 42;
    }
}
