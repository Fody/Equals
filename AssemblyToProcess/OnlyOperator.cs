[Equals(DoNotAddEquals = true, DoNotAddGetHashCode = true)]
public class OnlyOperator
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

    public static bool operator ==(OnlyOperator left, OnlyOperator right) => Operator.Weave();
    public static bool operator !=(OnlyOperator left, OnlyOperator right) => Operator.Weave();
}

/// Reproducing https://github.com/Fody/Equals/issues/99
public class UsingOnlyOperator
{
    public bool CompareNormalClassForEquality()
    {
        var left = new UsingOnlyOperator();
        var right = new UsingOnlyOperator();

        return left == right;
    }

    public bool CompareNormalClassForInequality()
    {
        var left = new UsingOnlyOperator();
        var right = new UsingOnlyOperator();

        return left != right;
    }
}