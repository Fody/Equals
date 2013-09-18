using System.Linq;

[Equals]
public class EmptyClass
{
    public bool ggggg()
    {
        int[] o = new int[0];

        return Enumerable.SequenceEqual(o, o);


    }
}
