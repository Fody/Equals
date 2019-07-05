[Equals]
public class InheritedClass : BaseClass
{
    public int B { get; set; }

    public static bool operator ==(InheritedClass left, InheritedClass right) => Operator.Weave();
    public static bool operator !=(InheritedClass left, InheritedClass right) => Operator.Weave();
}

[Equals]
public class BaseClass
{
    public int A { get; set; }

    public static bool operator ==(BaseClass left, BaseClass right) => Operator.Weave();
    public static bool operator !=(BaseClass left, BaseClass right) => Operator.Weave();
}