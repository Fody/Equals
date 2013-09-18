using System.Collections.Generic;

[Equals]
public class IntCollection
{
    public int Count { get; set; }

    public IEnumerable<int> Collection { get; set; }
}