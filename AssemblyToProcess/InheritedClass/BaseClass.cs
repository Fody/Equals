[Equals]
public class BaseClass
{
    public int A { get; set; }

    public static bool operator ==(BaseClass left, BaseClass right) => Operator.Weave();
    public static bool operator !=(BaseClass left, BaseClass right) => Operator.Weave();
}