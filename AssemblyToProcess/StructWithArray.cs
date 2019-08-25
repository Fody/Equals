[Equals]
public struct StructWithArray
{
    public int[] X { get; set; }
    public int[] Y { get; set; }

    public static bool operator ==(StructWithArray left, StructWithArray right) => Operator.Weave(left, right);
    public static bool operator !=(StructWithArray left, StructWithArray right) => Operator.Weave(left, right);
}