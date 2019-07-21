using System.Collections.Generic;

[Equals]
public class Child : Parent
{
    public long InChild { get; set; }

    public static bool operator ==(Child left, Child right) => Operator.Weave();
    public static bool operator !=(Child left, Child right) => Operator.Weave();
}

[Equals]
public class ComplexChild : ComplexParent
{
    public long InChildNumber { get; set; }

    public string InChildText { get; set; }

    public IEnumerable<int> InChildCollection { get; set; }

    public static bool operator ==(ComplexChild left, ComplexChild right) => Operator.Weave();
    public static bool operator !=(ComplexChild left, ComplexChild right) => Operator.Weave();
}

[Equals]
public class GenericChild : GenericParent<int>
{
    public string InChild { get; set; }

    public static bool operator ==(GenericChild left, GenericChild right) => Operator.Weave();
    public static bool operator !=(GenericChild left, GenericChild right) => Operator.Weave();
}
