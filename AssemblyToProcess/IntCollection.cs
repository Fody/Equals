using System.Collections.Generic;

[Equals]
public class IntCollection
{
    public int Count { get; set; }

    public IEnumerable<int> Collection { get; set; }

    public static bool operator ==(IntCollection left, IntCollection right) => Operator.Weave(left, right);
    public static bool operator !=(IntCollection left, IntCollection right) => Operator.Weave(left, right);
}