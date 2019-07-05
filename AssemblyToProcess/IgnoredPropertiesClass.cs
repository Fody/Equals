[Equals]
public class IgnoredPropertiesClass
{
    public int X { get; set; }

    [IgnoreDuringEquals]
    public int Y { get; set; }

    public static bool operator ==(IgnoredPropertiesClass left, IgnoredPropertiesClass right) => Operator.Weave();
    public static bool operator !=(IgnoredPropertiesClass left, IgnoredPropertiesClass right) => Operator.Weave();
}

[Equals]
public class InheritedIgnoredPropertiesClass : IgnoredPropertiesClass
{
    public static bool operator ==(InheritedIgnoredPropertiesClass left, InheritedIgnoredPropertiesClass right) => Operator.Weave();
    public static bool operator !=(InheritedIgnoredPropertiesClass left, InheritedIgnoredPropertiesClass right) => Operator.Weave();
}

