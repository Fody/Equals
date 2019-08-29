[Equals(IgnoreBaseClassProperties = true)]
public class IgnoreBaseClass :
    IgnoreBaseStubClass
{
    public int B { get; set; }

    public static bool operator ==(IgnoreBaseClass left, IgnoreBaseClass right) => Operator.Weave(left, right);
    public static bool operator !=(IgnoreBaseClass left, IgnoreBaseClass right) => Operator.Weave(left, right);
}