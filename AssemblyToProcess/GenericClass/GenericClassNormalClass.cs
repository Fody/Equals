[Equals]
public class GenericClassNormalClass :
    GenericClassBaseClass
{
    public int D { get; set; }

    public static bool operator ==(GenericClassNormalClass left, GenericClassNormalClass right) => Operator.Weave(left, right);
    public static bool operator !=(GenericClassNormalClass left, GenericClassNormalClass right) => Operator.Weave(left, right);
}