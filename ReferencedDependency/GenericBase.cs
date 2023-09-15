using System.Collections.Generic;
using System.Collections.ObjectModel;

public abstract class GenericBase<T>
{
    public ICollection<Item> Collection { get; set; } = new Collection<Item>();
}

public class Item;
