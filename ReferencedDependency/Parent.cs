using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReferencedDependency
{
    public class Parent
    {
        public long InParent { get; set; }
    }

    public class ComplexParent
    {
        public long InParentNumber { get; set; }

        public string InParentText { get; set; }

        public IEnumerable<int> InParentCollection { get; set; }
    }

    public class GenericParent<T>
    {
        public T GenericInParent { get; set; }
    }
}
