[Equals]
public class ClassWithIndexer
{
    public int X { get; set; }

    public byte Y { get; set; }

    public int this[int index]
    {
        get 
        {
            return X;
        }
        set 
        {
            X = index;
        }
    }
}
