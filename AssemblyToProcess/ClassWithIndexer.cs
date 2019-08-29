[Equals]
public class ClassWithIndexer
{
    public int X { get; set; }

    public byte Y { get; set; }

    public int this[int index]
    {
        get => X;
        set => X = index;
    }

    public static bool operator ==(ClassWithIndexer left, ClassWithIndexer right) => Operator.Weave(left, right);
    public static bool operator !=(ClassWithIndexer left, ClassWithIndexer right) => Operator.Weave(left, right);
}