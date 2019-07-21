[Equals]
public class EmptyClass
{
    public static bool operator ==(EmptyClass left, EmptyClass right) => Operator.Weave();
    public static bool operator !=(EmptyClass left, EmptyClass right) => Operator.Weave();
}