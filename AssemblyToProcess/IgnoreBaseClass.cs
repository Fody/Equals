[Equals(IgnoreBaseClassProperties = true)]
public class IgnoreBaseClass :
    IgnoreBaseStubClass
{
    public int B { get; set; }

    public static bool operator ==(IgnoreBaseClass left, IgnoreBaseClass right) => Operator.Weave();
    public static bool operator !=(IgnoreBaseClass left, IgnoreBaseClass right) => Operator.Weave();
}