[Equals(TypeCheck = TypeCheck.ExactlyOfType)]
public class ExactlyOfTypeClass
{
    public int A { get; set; }

    public static bool operator ==(ExactlyOfTypeClass left, ExactlyOfTypeClass right) => Operator.Weave();
    public static bool operator !=(ExactlyOfTypeClass left, ExactlyOfTypeClass right) => Operator.Weave();
}