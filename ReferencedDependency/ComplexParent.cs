using System.Collections.Generic;

public class ComplexParent
{
    public long InParentNumber { get; set; }

    public string InParentText { get; set; }

    public IEnumerable<int> InParentCollection { get; set; }
}