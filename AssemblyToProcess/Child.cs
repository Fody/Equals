using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ReferencedDependency;

[Equals]
public class Child : Parent
{
    public long InChild { get; set; }
}

[Equals]
public class ComplexChild : ComplexParent
{
    public long InChildNumber { get; set; }

    public string InChildText { get; set; }

    public IEnumerable<int> InChildCollection { get; set; }
}

[Equals]
public class GenericChild : GenericParent<int>
{
    public string InChild { get; set; }
}
