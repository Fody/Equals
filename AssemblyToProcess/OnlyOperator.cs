[Equals(DoNotAddEquals = true, DoNotAddGetHashCode = true)]
public class OnlyOperator
{
    public int Value { get; set; }

    public override bool Equals(object obj)
    {
        var second = obj as OnlyOperator;

        if (second == null)
        {
            return false;
        }

        return Value == 1 && second.Value == 2 || Value == 2 && second.Value == 1;
    }
}