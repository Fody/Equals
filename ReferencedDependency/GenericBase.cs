using System.Collections.Generic;
using System.Collections.ObjectModel;

public abstract class GenericBase<T>
{
    protected GenericBase()
    {
        Collection = new Collection<Item>();
    }

    public ICollection<Item> Collection { get; set; }
}

public class Item
{
}
