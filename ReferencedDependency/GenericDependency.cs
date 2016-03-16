using System.Collections;
using System.Collections.Generic;

public struct GenericDependency<T> : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator()
    {
        yield return (T)(object)Prop;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Prop { get; set; }
}
