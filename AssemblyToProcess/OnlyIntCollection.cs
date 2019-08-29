using System.Collections.Generic;

[Equals]
public class OnlyIntCollection
{
    public IEnumerable<int> Collection { get; set; }

    public static bool operator ==(OnlyIntCollection left, OnlyIntCollection right) => Operator.Weave(left, right);
    public static bool operator !=(OnlyIntCollection left, OnlyIntCollection right) => Operator.Weave(left, right);
}