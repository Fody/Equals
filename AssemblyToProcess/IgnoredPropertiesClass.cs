[Equals]
public class IgnoredPropertiesClass
{
    public int X { get; set; }

    [IgnoreDuringEquals]
    public int Y { get; set; }
}
