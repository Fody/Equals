[Equals]
public class StringArray
{
    public string[] Collection { get; set; }

    public static bool operator ==(StringArray left, StringArray right) => Operator.Weave(left, right);
    public static bool operator !=(StringArray left, StringArray right) => Operator.Weave(left, right);
}