using System.Collections.Generic;

[Equals]
public class ComplexChild :
    ComplexParent
{
    public long InChildNumber { get; set; }

    public string InChildText { get; set; }

    public IEnumerable<int> InChildCollection { get; set; }

    public static bool operator ==(ComplexChild left, ComplexChild right) => Operator.Weave();
    public static bool operator !=(ComplexChild left, ComplexChild right) => Operator.Weave();
}