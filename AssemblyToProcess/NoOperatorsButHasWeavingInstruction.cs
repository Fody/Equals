#pragma warning disable CS0660, CS0661
[Equals(DoNotAddEqualityOperators = true)]
public class NoOperatorsButHasWeavingInstruction
{
    public static bool operator ==(NoOperatorsButHasWeavingInstruction left, NoOperatorsButHasWeavingInstruction right) => Operator.Weave();
    public static bool operator !=(NoOperatorsButHasWeavingInstruction left, NoOperatorsButHasWeavingInstruction right) => Operator.Weave();
}