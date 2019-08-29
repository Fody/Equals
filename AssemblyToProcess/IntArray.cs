[Equals]
public class IntArray
{
    public int[] Collection { get; set; }

    public static bool operator ==(IntArray left, IntArray right) => Operator.Weave(left, right);
    public static bool operator !=(IntArray left, IntArray right) => Operator.Weave(left, right);
}