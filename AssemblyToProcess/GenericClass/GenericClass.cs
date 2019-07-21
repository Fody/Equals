using System.Collections.Generic;

[Equals]
public class GenericClass<T>
    where T : GenericClassBaseClass
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