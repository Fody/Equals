using System.Collections.Generic;

[Equals]
public class WithGenericParameter<T> : GenericClass<T> where T : GenericClassBaseClass
{
    public int X { get; set; }

    public static bool operator ==(WithGenericParameter<T> left, WithGenericParameter<T> right) => Operator.Weave();
    public static bool operator !=(WithGenericParameter<T> left, WithGenericParameter<T> right) => Operator.Weave();
}

[Equals]
public class WithoutGenericParameter : GenericClass<GenericClassBaseClass>
{
    public int Z { get; set; }

    public static bool operator ==(WithoutGenericParameter left, WithoutGenericParameter right) => Operator.Weave();
    public static bool operator !=(WithoutGenericParameter left, WithoutGenericParameter right) => Operator.Weave();
}

[Equals]
public class GenericClass<T> where T : GenericClassBaseClass
{
    public int a;

    public int A
    {
        get => a;
        set => a = value;
    }

    public IEnumerable<T> B { get; set; }

    public static bool operator ==(GenericClass<T> left, GenericClass<T> right) => Operator.Weave();
    public static bool operator !=(GenericClass<T> left, GenericClass<T> right) => Operator.Weave();
}

[Equals]
public abstract class GenericClassBaseClass
{
    public int C { get; set; }

    public static bool operator ==(GenericClassBaseClass left, GenericClassBaseClass right) => Operator.Weave();
    public static bool operator !=(GenericClassBaseClass left, GenericClassBaseClass right) => Operator.Weave();
}

[Equals]
public class GenericClassNormalClass : GenericClassBaseClass
{
    public int D { get; set; }

    public static bool operator ==(GenericClassNormalClass left, GenericClassNormalClass right) => Operator.Weave();
    public static bool operator !=(GenericClassNormalClass left, GenericClassNormalClass right) => Operator.Weave();
}

[Equals]
public class GenericProperty<T>
{
    public T Prop { get; set; }

    static bool Z(T a, T b)
    {
        return Equals(a, b);
    }

    public static bool operator ==(GenericProperty<T> left, GenericProperty<T> right) => Operator.Weave();
    public static bool operator !=(GenericProperty<T> left, GenericProperty<T> right) => Operator.Weave();
}