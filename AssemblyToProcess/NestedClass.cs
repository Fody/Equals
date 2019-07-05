[Equals]
public class NestedClass
{
    public int A { get; set; }

    public string B { get; set; }

    public double C { get; set; }

    public NormalClass D { get; set; }

    public static bool operator ==(NestedClass left, NestedClass right) => Operator.Weave();
    public static bool operator !=(NestedClass left, NestedClass right) => Operator.Weave();
}