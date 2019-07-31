[Equals(DoNotAddEquals = true, DoNotAddGetHashCode = true)]
public class OnlyOperatorUsingWeavingInstructionOverloadingWithParameters
{
    public int Value { get; set; }

    public override bool Equals(object obj)
    {
        if (!(obj is OnlyOperator second))
        {
            return false;
        }

        return Value == 1 && second.Value == 2;
    }

    public static bool operator ==(OnlyOperatorUsingWeavingInstructionOverloadingWithParameters left, OnlyOperatorUsingWeavingInstructionOverloadingWithParameters right) => Operator.Weave(left, right);
    public static bool operator !=(OnlyOperatorUsingWeavingInstructionOverloadingWithParameters left, OnlyOperatorUsingWeavingInstructionOverloadingWithParameters right) => Operator.Weave(left, right);
}