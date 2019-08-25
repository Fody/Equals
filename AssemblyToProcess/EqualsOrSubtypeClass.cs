[Equals(TypeCheck = TypeCheck.EqualsOrSubtype)]
public class EqualsOrSubtypeClass
{
    public int A { get; set; }

    public static bool operator ==(EqualsOrSubtypeClass left, EqualsOrSubtypeClass right) => Operator.Weave(left, right);
    public static bool operator !=(EqualsOrSubtypeClass left, EqualsOrSubtypeClass right) => Operator.Weave(left, right);
}