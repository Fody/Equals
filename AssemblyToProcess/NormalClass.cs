[Equals(DoNotAddEqualityOperators = true)]
public class NormalClass
{
    public int X { get; set; }

    public string Y { get; set; }

    public double Z { get; set; }

    public char V { get; set; }

    public static bool operator ==(NormalClass left, NormalClass right) => Operator.Weave();
    public static bool operator !=(NormalClass left, NormalClass right) => Operator.Weave();
}

/// Reproducing https://github.com/Fody/Equals/issues/99
public class UsingNormalClass
{
    public bool CompareNormalClassForEquality()
    {
        var left = new NormalClass();
        var right = new NormalClass();

        return left == right;
    }

    public bool CompareNormalClassForInequality()
    {
        var left = new NormalClass();
        var right = new NormalClass();

        return left != right;
    }
}
