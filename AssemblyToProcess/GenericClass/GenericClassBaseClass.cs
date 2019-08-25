[Equals]
public abstract class GenericClassBaseClass
{
    public int C { get; set; }

    public static bool operator ==(GenericClassBaseClass left, GenericClassBaseClass right) => Operator.Weave(left, right);
    public static bool operator !=(GenericClassBaseClass left, GenericClassBaseClass right) => Operator.Weave(left, right);
}