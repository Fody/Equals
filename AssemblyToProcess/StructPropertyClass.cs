[Equals]
public class StructPropertyClass
{
    public int A { get; set; }

    public SimpleStruct B { get; set; }

    public static bool operator ==(StructPropertyClass left, StructPropertyClass right) => Operator.Weave();
    public static bool operator !=(StructPropertyClass left, StructPropertyClass right) => Operator.Weave();
}