#pragma warning disable CS0660, CS0661
[Equals(DoNotAddEqualityOperators = true)]
public class OperatorsOptOutButHasWeavingInstruction
{
    public static bool operator ==(OperatorsOptOutButHasWeavingInstruction left, OperatorsOptOutButHasWeavingInstruction right) => Operator.Weave(left, right);
    public static bool operator !=(OperatorsOptOutButHasWeavingInstruction left, OperatorsOptOutButHasWeavingInstruction right) => Operator.Weave(left, right);
}