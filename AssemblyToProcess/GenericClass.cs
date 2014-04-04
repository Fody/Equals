using System.Collections.Generic;

[Equals]
public class WithGenericParameter<T> : GenericClass<T> where T : GenericClassBaseClass
{
   public int X { get; set; }
}

[Equals]
public class WithoutGenericParameter: GenericClass<GenericClassBaseClass> 
{
   public int Z { get; set; }
}

[Equals]
public class GenericClass<T> where T : GenericClassBaseClass
{
    public int a;

    public int A
    {
        get
        {
            return a;
        }
        set
        {
            a = value;
        }
    }

    public IEnumerable<T> B { get; set; }
}

[Equals]
public abstract class GenericClassBaseClass
{
    public int C { get; set; }
}

[Equals]
public class GenericClassNormalClass : GenericClassBaseClass
{
    public int D { get; set; }
}

[Equals]
public class GenericProperty<T>
{
    public T Prop { get; set; }

    static bool  Z(T a, T b)
    {
        return object.Equals(a,b);
    }
}