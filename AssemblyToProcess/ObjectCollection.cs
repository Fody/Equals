using System.Collections.Generic;

[Equals]
public class ObjectCollection
{
    public IEnumerable<object> Collection { get; set; }

    public static bool operator ==(ObjectCollection left, ObjectCollection right) => Operator.Weave();
    public static bool operator !=(ObjectCollection left, ObjectCollection right) => Operator.Weave();
}