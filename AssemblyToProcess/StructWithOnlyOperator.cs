[Equals(DoNotAddEquals = true, DoNotAddGetHashCode = true)]
public struct StructWithOnlyOperator
{
    public int Value { get; set; }

    public override bool Equals(object obj)
    {
        var second = (StructWithOnlyOperator)obj;

        return Value == 1 && second.Value == 2;
    }

    public static bool operator ==(StructWithOnlyOperator left, StructWithOnlyOperator right) => Operator.Weave();
    public static bool operator !=(StructWithOnlyOperator left, StructWithOnlyOperator right) => Operator.Weave();
}