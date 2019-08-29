[Equals]
public struct SimpleStruct
{
    public int X { get; set; }

    public int Y { get; set; }

    public static bool operator ==(SimpleStruct left, SimpleStruct right) => Operator.Weave(left, right);
    public static bool operator !=(SimpleStruct left, SimpleStruct right) => Operator.Weave(left, right);
}